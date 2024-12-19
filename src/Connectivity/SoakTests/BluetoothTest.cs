using System;
using System.Threading;
using System.Threading.Tasks;
using SoakTests.Common;

using Meadow;
using Meadow.Gateways.Bluetooth;

namespace SoakTests;

/// <summary>
/// Perform the async Bluetooth test.
/// </summary>
class BluetoothTest : ISoakTest
{
    /// <summary>
    /// Name of the On/Off characteristic.
    /// </summary>
    const string ON_OFF_NAME = "OnOff";

    /// <summary>
    /// UUID of the ON/Off characteristic.
    /// </summary>
    const string ON_OFF_UUID = "016e99d6-8a61-11eb-8dcd-0242ac1300ff";

    /// <summary>
    /// Name of the boolean characteristic.
    /// </summary>
    const string BOOLEAN_FIELD_NAME = "BooleanField";

    /// <summary>
    /// UUID of the boolean characteristic.
    /// </summary>
    const string BOOLEAN_FIELD_UUID = "017e99d6-8a61-11eb-8dcd-0242ac1300aa";

    /// <summary>
    /// Name of the number characteristic.
    /// </summary>
    const string NUMBER_FIELD_NAME = "NumberField";

    /// <summary>
    /// UUID of the number characteristic.
    /// </summary>
    const string NUMBER_FIELD_UUID = "018e99d6-8a61-11eb-8dcd-0242ac1300bb";

    /// <summary>
    /// Name of the text characteristic.
    /// </summary>
    const string TEXT_FIELD_NAME = "TextField";

    /// <summary>
    /// UUID of the text characteristic.
    /// </summary>
    const string TEXT_FIELD_UUID = "019e99d6-8a61-11eb-8dcd-0242ac1300cc";

    /// <summary>
    /// Definition of the Bluetooth service.
    /// </summary>
    Definition bleTreeDefinition;

    /// <summary>
    /// On/Off characteristic.
    /// </summary>
    CharacteristicBool onOffCharacteristic;

    /// <summary>
    /// Soak test configuration object.
    /// </summary>
    SoakTestSettings _config;

    /// <summary>
    /// Setup the test.
    /// </summary>
    /// <param name="config">General soak test configuration</param>
    public void Initialize(SoakTestSettings config)
    {
        _config = config;
        Resolver.Log.Info("Starting the BLE server.");

        bleTreeDefinition = GetDefinition();
        ((IF7MeadowDevice) Helpers.DeviceUnderTest).BluetoothAdapter.StartBluetoothServer(bleTreeDefinition);

        foreach (var characteristic in bleTreeDefinition.Services[0].Characteristics)
        {
            characteristic.ValueSet += (c, d) =>
            {
                Resolver.Log.Info($"Received data for Characteristic '{c.Name}' of type {d.GetType().Name}: {d}");
                switch (c.Name)
                {
                    case BOOLEAN_FIELD_NAME:
                        if (d is bool b)
                        {
                            c.SetValue(b);
                        }
                        break;
                    case NUMBER_FIELD_NAME:
                        c.SetValue(d);
                        break;
                    case TEXT_FIELD_NAME:
                        c.SetValue(d);
                        break;
                }
            };
        }

        onOffCharacteristic.ValueSet += (c, d) =>
        {
            Resolver.Log.Info($"{c.Name}: {d}");
            if (d is bool b)
            {
                c.SetValue(d);
            }
            bleTreeDefinition.Services[0].Characteristics[NUMBER_FIELD_NAME].SetValue(20000);
        };

        Resolver.Log.Info("Hardware initialized.");
    }

    /// <summary>
    /// Execute the test.
    /// </summary>
    public Task Execute()
    {
        //
        //  We need a slight pause before we can set a value.  This is necessary to allow the Bluetooth service to complete startup.
        //
        Thread.Sleep(2000);

        bleTreeDefinition.Services[0].Characteristics[ON_OFF_NAME].SetValue(true);
        bleTreeDefinition.Services[0].Characteristics[BOOLEAN_FIELD_NAME].SetValue(true);
        bleTreeDefinition.Services[0].Characteristics[NUMBER_FIELD_UUID].SetValue(42);
        bleTreeDefinition.Services[0].Characteristics[TEXT_FIELD_NAME].SetValue("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");

        Resolver.Log.Info("Data set.");
        //
        //  We just wait here and let the events deal with any data changes.
        //
        Thread.Sleep(Timeout.Infinite);
        //
        //  We should not get to the return statement but we need it to keep the compiler quiet.
        //
        return Task.CompletedTask;
    }

    /// <summary>
    /// Perform any necessary cleanup at the end of the test run.
    /// </summary>
    public void Teardown()
    {
    }

    /// <summary>
    /// Create a Bluetooth service consisting of 4 characteristics.
    /// </summary>
    /// <returns>Definition of the Bluetooth service.</returns>
    private Definition GetDefinition()
    {
        onOffCharacteristic = new CharacteristicBool(
                ON_OFF_NAME,
                uuid: ON_OFF_UUID,
                CharacteristicPermission.Read | CharacteristicPermission.Write,
                CharacteristicProperty.Read | CharacteristicProperty.Write);

        var service = new Service(
             "GattService",
             253,
             onOffCharacteristic,

             new CharacteristicBool(
                 BOOLEAN_FIELD_NAME,
                 uuid: BOOLEAN_FIELD_UUID,
                 permissions: CharacteristicPermission.Read,
                 properties: CharacteristicProperty.Read
                 ),

             new CharacteristicInt32(
                 NUMBER_FIELD_NAME,
                 uuid: NUMBER_FIELD_UUID,
                 permissions: CharacteristicPermission.Write | CharacteristicPermission.Read,
                 properties: CharacteristicProperty.Write | CharacteristicProperty.Read
                 ),

             new CharacteristicString(
                 TEXT_FIELD_NAME,
                 uuid: TEXT_FIELD_UUID,
                 maxLength: 256,
                 permissions: CharacteristicPermission.Write | CharacteristicPermission.Read,
                 properties: CharacteristicProperty.Write | CharacteristicProperty.Read
                 )
        );

        return new Definition("MeadowF7", service);
    }
}