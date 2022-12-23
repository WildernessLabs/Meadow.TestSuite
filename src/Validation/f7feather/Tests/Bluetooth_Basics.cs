﻿using Meadow;
using Meadow.Gateways.Bluetooth;
using System;
using System.Threading.Tasks;
using Validation;

namespace F7Feather.Tests
{
    internal class Bluetooth_Basics : ITestFeatherF7
    {
        Definition bleTreeDefinition;
        CharacteristicBool onOffCharacteristic;

        public Task<bool> RunTest(IF7MeadowDevice device)
        {
            Console.WriteLine("Initialize hardware...");

            // initialize the bluetooth defnition tree
            Console.WriteLine("Starting the BLE server.");
            bleTreeDefinition = GetDefinition();
            device.BluetoothAdapter.StartBluetoothServer(bleTreeDefinition);

            // wire up some notifications on set
            foreach (var characteristic in bleTreeDefinition.Services[0].Characteristics)
            {
                characteristic.ValueSet += (c, d) =>
                {
                    Console.WriteLine($"HEY, I JUST GOT THIS BLE DATA for Characteristic '{c.Name}' of type {d.GetType().Name}: {d}");
                };
            }

            // addressing individual characteristics:
            onOffCharacteristic.ValueSet += (c, d) =>
            {
                Console.WriteLine($"{c.Name}: {d}");
            };

            Console.WriteLine("Hardware initialized.");

            return Task.FromResult(true);
        }

        protected Definition GetDefinition()
        {
            onOffCharacteristic = new CharacteristicBool(
                    "On_Off",
                    Guid.NewGuid().ToString(),
                    CharacteristicPermission.Read | CharacteristicPermission.Write,
                    CharacteristicProperty.Read | CharacteristicProperty.Write);

            var service = new Service(
                 "ServiceA",
                 253,
                 onOffCharacteristic,

                 new CharacteristicBool(
                     "My Bool",
                     uuid: "017e99d6-8a61-11eb-8dcd-0242ac1300aa",
                     permissions: CharacteristicPermission.Read,
                     properties: CharacteristicProperty.Read
                     ),

                 new CharacteristicInt32(
                     "My Number",
                     uuid: "017e99d6-8a61-11eb-8dcd-0242ac1300bb",
                     permissions: CharacteristicPermission.Write | CharacteristicPermission.Read,
                     properties: CharacteristicProperty.Write | CharacteristicProperty.Read
                     ),

                 new CharacteristicString(
                     "My Text",
                     uuid: "017e99d6-8a61-11eb-8dcd-0242ac1300cc",
                     maxLength: 20,
                     permissions: CharacteristicPermission.Write | CharacteristicPermission.Read,
                     properties: CharacteristicProperty.Write | CharacteristicProperty.Read
                     )
            );

            return new Definition("MY MEADOW F7", service);
        }
    }
}
