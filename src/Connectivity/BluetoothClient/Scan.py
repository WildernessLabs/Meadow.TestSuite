#!/usr/bin/env python

'''
The purpose of this script is to scan a named device (or address) and list the services and characteristics.
'''

import argparse
import asyncio
import logging
import sys

from bleak import BleakClient, BleakScanner

DEFAULT_DEVICE_NAME = 'MeadowF7'

logger = logging.getLogger(__name__)

async def main(args: argparse.Namespace):
    global client_number_value
    logger.info('Starting scan...')

    if args.address:
        device = await BleakScanner.find_device_by_address(args.address, cb = dict(use_bdaddr = args.macos_use_bdaddr))
        if device is None:
            logger.error(f'Could not find device with address {args.address}')
            return
    else:
        device_name = args.name if args.name else DEFAULT_DEVICE_NAME
        device = await BleakScanner.find_device_by_name(device_name, cb = dict(use_bdaddr = args.macos_use_bdaddr))
        if device is None:
            logger.error(f'Could not find device with name {device_name}')
            return

    logger.info('Connecting to device...')
    async with BleakClient(device, services = args.services) as client:
        logger.info('Connected')
        for service in client.services:
            logger.info(f'Service: {service.description} ({service.uuid})')
            for characteristic in service.characteristics:
                characteristic_value = None
                if 'read' in characteristic.properties or 'notify' in characteristic.properties:
                    try:
                        characteristic_value = bytes(await client.read_gatt_char(characteristic))
                    except Exception as e:
                        characteristic_value = None
                properties = ', '.join(characteristic.properties)
                logger.info(f'     Characteristic: {characteristic.description} ({characteristic.uuid}), properties: {properties}, value: {characteristic_value}')
                for descriptor in characteristic.descriptors:
                    descriptor_value = None
                    attempts = 0
                    while attempts < 10:
                        try:
                            descriptor_value = await client.read_gatt_descriptor(descriptor)
                            if descriptor_value:
                                break
                        except Exception as e:
                            descriptor_value = None
                        attempts += 1
                    if descriptor_value is not None:
                        descriptor_value = bytes(descriptor_value)
                    logger.info(f'        Descriptor: {descriptor.description} ({descriptor.uuid}), value: {descriptor_value}')
    logger.info('Disconnected')

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

    try:
        asyncio.run(main(args))
    except Exception as e:
        logger.error('Error occurred: %s', e)