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
/// <remarks>
/// This test run will be controlled by the client application running on a host computer.
/// The application on the host machine is expected to perform the following steps:
///     - Set the numeric characteristic value.  This application will then set the value on the ESP32.
///     - The host application will check that the value that has just been set matches the value this application has set on the ESP32.
/// This will repeat until the value set by the application on the host computer is 0.
/// </remarks>
class BluetoothNumericTest : ISoakTest
{
    /// <summary>
    /// Name of the number characteristic.
    /// </summary>
    const string NUMBER_FIELD_NAME = "NumberField";

    /// <summary>
    /// UUID of the number characteristic.
    /// </summary>
    const string NUMBER_FIELD_UUID = "018e99d6-8a61-11eb-8dcd-0242ac1300bb";

    /// <summary>
    /// Definition of the Bluetooth service.
    /// </summary>
    Definition bleTreeDefinition;

    /// <summary>
    /// Soak test configuration object.
    /// </summary>
    SoakTestSettings _config;

    /// <summary>
    /// 
    /// </summary>
    CharacteristicInt32 _numberCharacteristic;

    /// <summary>
    /// Semaphore used to indicate if a test is still running.
    /// </summary>
    private static SemaphoreSlim _semaphore = new SemaphoreSlim(0, 1);

    /// <summary>
    /// Setup the test.
    /// </summary>
    /// <param name="config">General soak test configuration</param>
    public void Initialize(SoakTestSettings config)
    {
        _config = config;
        Resolver.Log.Info("Starting the BLE server.");

        _numberCharacteristic = new CharacteristicInt32(NUMBER_FIELD_NAME,
                 uuid: NUMBER_FIELD_UUID,
                 permissions: CharacteristicPermission.Write | CharacteristicPermission.Read,
                 properties: CharacteristicProperty.Write | CharacteristicProperty.Read
                 );
        var service = new Service(
             "GattService",
             253,

             _numberCharacteristic
        );
        bleTreeDefinition = new Definition("MeadowF7", service);
        ((IF7MeadowDevice) Helpers.DeviceUnderTest).BluetoothAdapter.StartBluetoothServer(bleTreeDefinition);
        _numberCharacteristic.ValueSet += (c, d) =>
        {
            Resolver.Log.Info($"Received data for Characteristic '{c.Name}' of type {d.GetType().Name}: {d}");
            if (d is int i)
            {
                c.SetValue(i);
                if (i == 0)
                {
                    _semaphore.Release();
                }
            }
            else
            {
                throw new ArgumentException("Data is not an integer.");
            }
        };

        Resolver.Log.Info("Hardware initialized.");
    }

    /// <summary>
    /// Execute the test.
    /// </summary>
    public Task Execute()
    {
        //
        //  We wait here until the characteristic value is set to 0.  This will be set by the host application
        //  and echoed here through the ValueSet event attached to the numeric characteristic.
        //
        _semaphore.Wait();

        //
        //  We should not get to the return statement but we need it to keep the compiler quiet.
        //
        Resolver.Log.Info("Test completed.");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Perform any necessary cleanup at the end of the test run.
    /// </summary>
    public void Teardown()
    {
    }
}