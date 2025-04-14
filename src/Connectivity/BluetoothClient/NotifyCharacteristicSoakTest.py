#!/usr/bin/env python

'''
The purpose of this script is to test the notification properties in Meadow.Core.

How to use this script:
1 - Edit the app.config.yaml in the ProjLabV3SoakTestRunner project setting the TestName to "BluetoothNotifyTest"
2 - Deploy the ProjLabV3SoakTestRunner application to the Meadow device.
3 - On a Raspberry Pi board with Bleak installed, run this script.
'''

import argparse
import asyncio
import logging
import sys

from bleak import BleakClient, BleakScanner

DEFAULT_DEVICE_NAME = 'MeadowF7NotifyTest'

ITERATION_COUNT_NAME = 'IterationCountField'
ITERATION_COUNT_UUID = '018e99d6-8a61-11eb-8dcd-0242ac1300aa'
iteration_characteristic = None

CLIENT_NUMBER_FIELD_NAME = 'ClientNumberField'
CLIENT_NUMBER_FIELD_UUID = '018e99d6-8a61-11eb-8dcd-0242ac1300bb'
client_number_characteristic = None
client_number_value = 0xffffffff

NOTIFY_FIELD_NAME = 'NotifyField'
NOTIFY_FIELD_UUID = '018e99d6-8a61-11eb-8dcd-0242ac1300cc'
notify_characteristic = None

connected_client = None

NUMBER_OF_ITERATIONS = 10000

logger = logging.getLogger(__name__)

async def find_characteristics(service):
    """
    Find the characteristics we are interested in.

    :param service: The service to search for characteristics in.
    """
    global iteration_characteristic
    global client_number_characteristic
    global notify_characteristic

    for characteristic in service.characteristics:
        if (characteristic.uuid == ITERATION_COUNT_UUID):
            iteration_characteristic = characteristic
        elif characteristic.uuid == CLIENT_NUMBER_FIELD_UUID:
            client_number_characteristic = characteristic
        elif characteristic.uuid == NOTIFY_FIELD_UUID:
            notify_characteristic = characteristic
            logger.info(f'Notify characteristic properties: {notify_characteristic.properties}')

async def set_characteristic_value(client, characteristic, value):
    """
    Set the value of a characteristic and verify the value.

    :param client: The client to use.
    :param characteristic: The characteristic to set.
    :param value: The value.
    """
    bytes = value.to_bytes(4, byteorder = 'little')
    await client.write_gatt_char(characteristic.uuid, bytes)
    attempt = 0
    while attempt < 10:
        response = await client.read_gatt_char(characteristic.uuid)
        if value == int.from_bytes(response, byteorder = 'little'):
            break
        attempt += 1
    if value != int.from_bytes(response, byteorder = 'little'):
        raise Exception('[Read] {characteristic}, Error: Value mismatch (Iteration {value})')
        
async def display_progress_counter(counter):
    """
    Display progress counter.

    :param counter: The counter.
    """
    if (counter < 10) or (counter % 100 == 0):
        logger.info('    Iteration: %d', counter)

async def notify_handler(handle: int, data: bytearray):
    """
    Notify handler.

    :param sender: The sender.
    :param data: The data.
    """
    global connected_client
    global client_number_characteristic
    global client_number_value
    client_number_value = int.from_bytes(data, byteorder = 'little')
    if client_number_value == 0:
        return
    await connected_client.write_gatt_char(client_number_characteristic.uuid, data)
    await display_progress_counter(client_number_value)

async def main(args: argparse.Namespace):
    global client_number_value
    global connected_client
    logger.info('Starting scan...')

    if args.address:
        device = await BleakScanner.find_device_by_address(args.address, cb = dict(use_bdaddr = args.macos_use_bdaddr))
        if device is None:
            logger.error('Could not find device with address %s', args.address)
            return
    else:
        device_name = args.name if args.name else DEFAULT_DEVICE_NAME
        device = await BleakScanner.find_device_by_name(device_name, cb = dict(use_bdaddr = args.macos_use_bdaddr))
        if device is None:
            logger.error('Could not find device with name %s', device_name)
            return

    logger.info('Connecting to device...')
    async with BleakClient(device, services = args.services) as client:
        logger.info('Connected')
        connected_client = client
        for service in client.services:
            await find_characteristics(service)
        if (iteration_characteristic is None) or (client_number_characteristic is None) or (notify_characteristic is None):
            logger.error('Could not find all of the characteristics')
            return
        #
        #   Setting this characteristic to 1 before the number of iterations will tell the application running on
        #   the Meadow that this test is controlled by a script.
        #
        await client.start_notify(NOTIFY_FIELD_UUID, notify_handler)
        await set_characteristic_value(client, client_number_characteristic, 0x7fffffff)
        await set_characteristic_value(client, iteration_characteristic, NUMBER_OF_ITERATIONS)
        logger.info('Waiting for test to end...')
        #
        #   not the most pleasant loop but it works.
        #
        while client_number_value != 0:
            await asyncio.sleep(1)
        logger.info('Disconnecting...')
    logger.info('Disconnected')
    logger.info('Completed notify soak test.')

if __name__ == '__main__':
    if sys.platform.startswith("darwin"):
        logger.error('This script does not work correctly on MacOS')
        sys.exit(1)

    parser = argparse.ArgumentParser()

    device_group = parser.add_mutually_exclusive_group(required = False)
    device_group.add_argument('--name', metavar = '<name>', help = 'the name of the bluetooth device to connect to')
    device_group.add_argument('--address', metavar = '<address>', help = 'the address of the bluetooth device to connect to')

    parser.add_argument('--macos-use-bdaddr', action = 'store_true', help = 'when true use Bluetooth address instead of UUID on macOS')
    parser.add_argument('--services', nargs = '+', metavar = '<uuid>', help = 'if provided, only enumerate matching service(s)')
    parser.add_argument('-d', '--debug', action = 'store_true', help = 'sets the log level to debug')

    args = parser.parse_args()

    log_level = logging.DEBUG if args.debug else logging.INFO
    logging.basicConfig(level = log_level, format = '%(asctime)-15s %(name)-8s %(levelname)s: %(message)s')

    asyncio.run(main(args))