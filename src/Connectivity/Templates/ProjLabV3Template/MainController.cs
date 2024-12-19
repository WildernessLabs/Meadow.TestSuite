using ProjectLabTest.Hardware;
using ProjectLabTest.Services;
using Meadow.Foundation;
using System;
using System.Threading;

namespace ProjectLabTest
{
    internal class MainController
    {
        IProjectLabV3Hardware _hardware;

        DisplayController _displayService;

        public MainController(IProjectLabV3Hardware hardware)
        {
            this._hardware = hardware;
        }

        public void Initialize()
        {
            _hardware.Initialize();

            _displayService = new DisplayController(_hardware.Display);

            string title = "Testing";

            string[] text = new string[]
            {
                "",  "", "", "", "", "", "", "", "", "", "", "", "", ""
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
                Thread.Sleep(500);
            }

            Thread.Sleep(Timeout.Infinite);
        }
    }
}