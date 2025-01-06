#!/usr/bin/env python

'''
The purpose of this script is to repeatedly write a number to a characteristic and read it back to verify the value.
It is designed to test code running on a Meadow device and the characteristic UUIDs in this file match the UUIDs
in the Meadow managed code in Bluetooth_Basics.
'''

import argparse
import asyncio
import logging

from bleak import BleakClient, BleakScanner

DEFAULT_DEVICE_NAME = 'MeadowF7'

ON_OFF_NAME = 'OnOff'
ON_OFF_UUID = '016e99d6-8a61-11eb-8dcd-0242ac1300ff'

BOOLEAN_FIELD_NAME = 'BooleanField'
BOOLEAN_FIELD_UUID = '017e99d6-8a61-11eb-8dcd-0242ac1300aa'

NUMBER_FIELD_NAME = 'NumberField'
NUMBER_FIELD_UUID = '018e99d6-8a61-11eb-8dcd-0242ac1300bb'

TEXT_FIELD_NAME = 'TextField'
TEXT_FIELD_UUID = '019e99d6-8a61-11eb-8dcd-0242ac1300cc'

NUMBER_OF_ITERATIONS = 10000

logger = logging.getLogger(__name__)

async def main(args: argparse.Namespace):
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
        for service in client.services:
            for char in service.characteristics:
                if char.uuid == NUMBER_FIELD_UUID:
                    logger.info('Starting write / read soak test.')
                    counter = 0
                    while counter < NUMBER_OF_ITERATIONS:
                        counter += 1
                        if counter % 100 == 0:
                            logger.info('    Iteration: %d', counter)
                        try:
                            bytes = counter.to_bytes(4, byteorder = 'little')
                            await client.write_gatt_char(char.uuid, bytes)
                            attempt = 0
                            while True:
                                response = await client.read_gatt_char(char.uuid)
                                if counter == int.from_bytes(response, byteorder = 'little'):
                                    break
                                attempt += 1
                                if attempt > 10:
                                    break
                            if counter != int.from_bytes(response, byteorder = 'little'):
                                logger.error('    [Read] %s, Error: Value mismatch (Iteration %d)', char, counter)
                                break
                        except Exception as e:
                            logger.error('    Iteration: {counter:,}')
                            logger.error('    [Write] %s, Error: %s', char, e)
                            break
        logger.info('Disconnecting...')
    logger.info('Disconnected')
    logger.info('Completed write / read soak test.')

if __name__ == '__main__':
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