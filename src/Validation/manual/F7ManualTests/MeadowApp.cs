using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using System;
using System.Threading.Tasks;

namespace F7ManualTests
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV2>
    {
        private int _start;

        public override async Task Run()
        {
            Resolver.Log.Info("Run...");

            var output = Device.Pins.D03.CreateDigitalOutputPort(false);
            output.State = true; // ensure the set stuff is interpreted
            output.State = false;

            var input = Device.Pins.D04.CreateDigitalInterruptPort(InterruptMode.EdgeRising, ResistorMode.InternalPullDown);
            input.Changed += OnInputChanged;

            OnInputChanged(null, new DigitalPortResult());

            for (var i = 0; i < 5; i++)
            {
                _start = Environment.TickCount;
                output.State = true;
                await Task.Delay(1000);
                output.State = false;
                await Task.Delay(1000);
            }

        }

        private void OnInputChanged(object sender, DigitalPortResult e)
        {
            if (sender == null)
            {
                Resolver.Log.Info($"primed");
                return;
            }

            var et = Environment.TickCount - _start;

            Resolver.Log.Info($"Interrupt took {et} ms");
        }
    }
}