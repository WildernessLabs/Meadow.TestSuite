using Meadow.Hardware;
using Meadow.Devices;
using Meadow.Foundation;
using System;
using System.Threading;
using FeatherF7Test.Hardware;
using FeatherF7Test.Services;

using Meadow.Foundation.Leds;

namespace FeatherF7Test
{
    internal class MainController
    {
        F7FeatherBase _device;

        IOLEDBoardHardware _hardware;

        DisplayController _displayService;

        public MainController(F7FeatherBase device)
        {
            _device = device;
        }

        public void Initialize()
        {
            _hardware = new OLEDBoardHardware();
            _hardware.Initialize(_device);

            _displayService = new DisplayController(_hardware.Display);

            string title = "Testing";

            string[] text = new string[]
            {
                "",  ""
            };

            _displayService.UpdateTitle(title);
            _displayService.UpdateText(text);
        }

        public void Run()
        {
            //
            //  Test code goes here.
            //
            for (int index = 0; index < 100; index++)
            {
                _displayService.Log(index.ToString());
                _hardware.Led1.IsOn = !_hardware.Led1.IsOn;
                // _hardware.Led2.IsOn = !_hardware.Led2.IsOn;
                // _hardware.Led3.IsOn = !_hardware.Led3.IsOn;
                // _hardware.Led4.IsOn = !_hardware.Led4.IsOn;
                Thread.Sleep(500);
            }

            Thread.Sleep(Timeout.Infinite);
        }
    }
}