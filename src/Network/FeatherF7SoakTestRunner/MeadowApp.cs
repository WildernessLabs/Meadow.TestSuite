using FeatherF7Test.Hardware;
using FeatherF7Test.Services;
using Meadow;
using Meadow.Devices;
using System;
using System.Threading;
using System.Threading.Tasks;

using SoakTests;
using SoakTests.Common;

namespace FeatherF7Test
{
    // public class MeadowApp : App<F7FeatherV1>
    public class MeadowApp : App<F7FeatherV2>
    {
        /// <summary>
        /// 
        /// </summary>
        IOLEDBoardHardware _hardware;

        /// <summary>
        /// 
        /// </summary>
        DisplayController _displayService;

        /// <summary>
        /// 
        /// </summary>
        SoakTestSettings _config;

        /// <summary>
        /// 
        /// </summary>
        ISoakTest _test;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Task Initialize()
        {
            _config = new SoakTestSettings();

            _hardware = new OLEDBoardHardware();
            _hardware.Initialize(Device);

            _displayService = new DisplayController(_hardware.Display);
            _displayService.Clear();

            Helpers.WaitForNetworkConnection(Device);

            _test = RegisteredTests.GetTest(_config.TestName);
            _displayService.UpdateTitle(_test.Name);
            _test.Initialize(_config);

            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        public override Task Run()
        {
            //
            //  Now some variables used to show progress.
            //
            int counter = 0;
            int modulo = _config.NumberOfCycles switch
            {
                > 100 => 100,
                > 10 => 10,
                _ => 0
            };

            while (counter < _config.NumberOfCycles)
            {
                counter++;
                if ((modulo == 0) || (counter % modulo == 0))
                {
                    _displayService.Log($"{counter}");
                }
                _test.Execute();
                if (_config.DelayBetweenCyclesMs > 0)
                {
                    Thread.Sleep(_config.DelayBetweenCyclesMs);
                }
                _hardware.Led1.IsOn = !_hardware.Led1.IsOn;
            }
            _test.Teardown();

            _displayService.Log("Done.");

            Thread.Sleep(Timeout.Infinite);

            return Task.CompletedTask;
        }
    }
}