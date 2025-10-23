using System;
using System.Threading;
using System.Threading.Tasks;
using SoakTests.Common;

using Meadow;
using Meadow.Gateways.Bluetooth;

namespace SoakTests;

/// <summary>
/// Perform the async Bluetooth notification test.
/// </summary>
/// <remarks>
/// This test run will be controlled by the client application running on a host computer.
/// The application on the host machine is expected to perform the following steps:
///     - Set the iteration count characteristic value.  This application will then set the value on the ESP32.
///     - The host application will check that the value that has just been set matches the value this application has set on the ESP32.
/// This will repeat until the value set by the application on the host computer is 0.
/// </remarks>
class BluetoothNotifyTest : ISoakTest
{
    /// <summary>
    /// Name of the iteration count characteristic.
    /// </summary>
    const string ITERATION_COUNT_FIELD_NAME = "IterationCountField";

    /// <summary>
    /// UUID of the iteration count characteristic.
    /// </summary>
    const string ITERATION_COUNT_FIELD_UUID = "018e99d6-8a61-11eb-8dcd-0242ac1300aa";

    /// <summary>
    /// Name of the client number characteristic.
    /// </summary>
    const string CLIENT_NUMBER_FIELD_NAME = "ClientNumberField";

    /// <summary>
    /// UUID of the client number characteristic.
    /// </summary>
    const string CLIENT_NUMBER_FIELD_UUID = "018e99d6-8a61-11eb-8dcd-0242ac1300bb";

    /// <summary>
    /// Name of the notify characteristic.
    /// </summary>
    const string NOTIFY_FIELD_NAME = "NotifyField";

    /// <summary>
    /// UUID of the notify characteristic.
    /// </summary>
    const string NOTIFY_FIELD_UUID = "018e99d6-8a61-11eb-8dcd-0242ac1300cc";

    /// <summary>
    /// Definition of the Bluetooth service.
    /// </summary>
    Definition bleTreeDefinition;

    /// <summary>
    /// Soak test configuration object.
    /// </summary>
    SoakTestSettings _config;

    /// <summary>
    /// Iteration count characteristic.
    /// </summary>
    CharacteristicInt32 _iterationCountCharacteristic;

    /// <summary>
    /// Client number characteristic
    /// </summary>
    CharacteristicInt32 _clientNumberCharacteristic;

    /// <summary>
    /// Notification characteristic.
    /// </summary>
    CharacteristicInt32 _notifyCharacteristic;

    /// <summary>
    /// Semaphore used to indicate if a test is still running.
    /// </summary>
    private static SemaphoreSlim _semaphore = new SemaphoreSlim(0, 1);

    /// <summary>
    /// Number of iterations to be run.
    /// </summary>
    private static int _numberOfIterations = 0;

    /// <summary>
    /// Is this a scripted test?
    /// </summary>
    /// <remarks>
    /// </remarks>
    private static bool _scriptedTest = false;

    /// <summary>
    /// Client characteristic value as a local integer.
    /// </summary>
    private static int _clientNumber = 0;

    /// <summary>
    /// Setup the test.
    /// </summary>
    /// <param name="config">General soak test configuration</param>
    public void Initialize(SoakTestSettings config)
    {
        _config = config;
        Resolver.Log.Info("Starting the BLE server.");

        _iterationCountCharacteristic = new CharacteristicInt32(ITERATION_COUNT_FIELD_NAME,
                 uuid: ITERATION_COUNT_FIELD_UUID,
                 permissions: CharacteristicPermission.Write | CharacteristicPermission.Read,
                 properties: CharacteristicProperty.Write | CharacteristicProperty.Read
                 );
        _clientNumberCharacteristic = new CharacteristicInt32(CLIENT_NUMBER_FIELD_NAME,
                 uuid: CLIENT_NUMBER_FIELD_UUID,
                 permissions: CharacteristicPermission.Write | CharacteristicPermission.Read,
                 properties: CharacteristicProperty.Write | CharacteristicProperty.Read
                 );
        _notifyCharacteristic = new CharacteristicInt32(NOTIFY_FIELD_NAME,
                 uuid: NOTIFY_FIELD_UUID,
                 permissions: CharacteristicPermission.Write | CharacteristicPermission.Read,
                 properties: CharacteristicProperty.Write | CharacteristicProperty.Read | CharacteristicProperty.Notify
                 );
        var service = new Service(
             "GattService",
             253,

             _iterationCountCharacteristic,
             _clientNumberCharacteristic,
             _notifyCharacteristic
        );
        bleTreeDefinition = new Definition("MeadowF7NotifyTest", service);
        ((IF7MeadowDevice) Helpers.DeviceUnderTest).BluetoothAdapter.StartBluetoothServer(bleTreeDefinition);
        _clientNumberCharacteristic.SetValue(0);
        _clientNumberCharacteristic.ValueSet += ClientNumberCharacteristic_Changed;
        _iterationCountCharacteristic.ValueSet += (c, d) =>
        {
            if (d is int i)
            {
                _numberOfIterations = i;
                c.SetValue(i);
                _semaphore.Release();
            }
            else
            {
                throw new ArgumentException("Data is not an integer.");
            }
        };
        Resolver.Log.Info("BLE server started.");
        Resolver.Log.Info($"");
        Resolver.Log.Info($"********** Instructions **********");
        Resolver.Log.Info($"");
        Resolver.Log.Info($"This test can be run in two modes:");
        Resolver.Log.Info($"    1. Scripted");
        Resolver.Log.Info($"    2. Visual verification (default)");
        Resolver.Log.Info($"");
        Resolver.Log.Info($"In scripted mode a client script should be run to control the execution of the test.");
        Resolver.Log.Info($"");
        Resolver.Log.Info($"In Visual verification mode a client application should be connected to the Meadow board and the output monitored.");
        Resolver.Log.Info($"In this mode it is necessary to set the number of iterations to be executed in the characteristic with UUID {ITERATION_COUNT_FIELD_UUID}.");
        Resolver.Log.Info($"Please set the {ITERATION_COUNT_FIELD_UUID} characteristic now if you are running in this mode.");
        Resolver.Log.Info($"");
        Resolver.Log.Info($"Hint: The characteristic can be identified by checking the last two characters of the UUID.");
        Resolver.Log.Info($"");
        Resolver.Log.Info($"**********************************");
    }

    /// <summary>
    /// Handle the case when the client number characteristic is changed.
    /// </summary>
    /// <param name="sender">CharacteristicInt32 object.</param>
    /// <param name="d">Value that has been used to set the characteristic.</param>
    private void ClientNumberCharacteristic_Changed(object sender, object d)
    {
        CharacteristicInt32 c = (CharacteristicInt32) sender;
        if (d is int i)
        {
            if (i == 0x7fffffff)
            {
                Resolver.Log.Info($"Test will now run in scripted mode.");
                _scriptedTest = true;
            }
            c.SetValue(i);
            _clientNumber = i;
        }
        else
        {
            throw new ArgumentException("Data is not an integer.");
        }
    }

    /// <summary>
    /// Execute the test.
    /// </summary>
    public Task Execute()
    {
        //
        //  Wait until the client application has told us how many iterations we need to run.
        //
        _semaphore.Wait();
        int delay = 2000;
        Resolver.Log.Info($"Number of iterations to run: {_numberOfIterations}");
        if (!_scriptedTest)
        {
            Resolver.Log.Info($"Please ensure that the client is subscribed to notifications for the characteristic with UUID {NOTIFY_FIELD_UUID}.");
            _clientNumberCharacteristic.ValueSet -= ClientNumberCharacteristic_Changed;
            delay = 10000;
        }
        Resolver.Log.Info($"Test will start in {delay / 1000} seconds.");
        Task.Delay(delay).Wait();

        Resolver.Log.Info($"Starting the test.");
        _clientNumber = -1;
        for (int iteration = 1; iteration <= _numberOfIterations; iteration++)
        {
            if ((iteration < 10) || (iteration % 100 == 0))
            {
                Resolver.Log.Info($"    Iteration {iteration}.");
            }
            _notifyCharacteristic.SetValue(iteration);
            if (_scriptedTest)
            {
                int retryCount = 0;
                while (retryCount < 20)
                {
                    if (_clientNumber == iteration)
                    {
                        break;
                    }
                    Task.Delay(50).Wait();
                    retryCount++;
                }
                if (_clientNumber != iteration)
                {
                    Resolver.Log.Error($"Number read does not match iteration count: {_clientNumber} != {iteration}");
                    throw new Exception("Bluetooth characteristic notification test failed.");
                }
            }
        }
        _notifyCharacteristic.SetValue(0);
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