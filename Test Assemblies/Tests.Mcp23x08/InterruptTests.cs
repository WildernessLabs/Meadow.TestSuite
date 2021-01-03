using Meadow;
using Meadow.Devices;
using Meadow.Foundation.ICs.IOExpanders;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Hardware;
using Meadow.TestSuite;
using Mono.CodeGeneration;
using Munit;
using System;
using System.Threading;

namespace Peripheral
{
    public class InterruptTests
    {
        // DEV NOTE
        // with Meadow b4.2 (likely anything pre-AOT) the first interrupt handler is way slow, so we need to account for that during testing
        private static bool VeryFirstInterrupt { get; set; } = true;

        public bool ShowDebug { get; set; } = true;
        public IIODevice Device { get; set; }

        private const string SCLIdentifier = "D08";
        private const string SDAIdentifier = "D07";
        private const string InterruptIdentifier = "D06";

        [Fact]
        public void InterruptCheck()
        {
            // Get the Meadow pins we'll use
            Output.WriteLineIf(ShowDebug, "Getting pins...");
            var scl = Device.GetPin(SCLIdentifier);
            var sda = Device.GetPin(SDAIdentifier);
            var int_pin = Device.GetPin(InterruptIdentifier);
            Assert.NotNull(scl);
            Assert.NotNull(sda);
            Assert.NotNull(int_pin);

            // create the IIC bus
            Output.WriteLineIf(ShowDebug, "Creating IIC Bus...");
//            var bus = (Device as F7Micro).CreateI2cBus();
            var bus = Device.CreateI2cBus(scl, sda);
            Assert.NotNull(bus);

            var meadow_interrupt_count = 0;

            // this is the meadow interrupt that will forward expander interrupts
            using (var int_port = Device.CreateDigitalInputPort(
                int_pin,
                InterruptMode.EdgeRising,
                ResistorMode.PullDown,
                20,
                20))
            {
                int_port.Changed += (s, e) =>
               {
                   meadow_interrupt_count++;
                   Output.WriteLineIf(ShowDebug, $"Meadow Interrupt");
               };

                // build the expander
                Output.WriteLineIf(ShowDebug, "Creating the expander...");
                var expander = new Mcp23x08(bus, interruptPort: int_port);
                Assert.NotNull(expander, "Failed to create the Expander object");
                expander.InputChanged += (sender, args) =>
                {
                    Output.WriteLineIf(ShowDebug, $"! Expander Input Changed state: {Convert.ToString(args.InputState, 2)}");
                };

                // BUG BUG BUG - we don't pass this currently
                Assert.Equal(0, meadow_interrupt_count, "We got an unsolicted interrupt from the MCP ctor");

                Output.WriteLineIf(ShowDebug, "Creating the expanded input...");
                var gp_input = expander.CreateDigitalInputPort(expander.Pins.GP7, InterruptMode.EdgeBoth, ResistorMode.PullUp);
                gp_input.Changed += (sender, args) =>
                {
                    // dev note
                    // BUG BUG BUG
                    // Without the gp_input.State read here, this event will never fire a second time
                    Output.WriteLineIf(ShowDebug, $"Expanded input interrupt. Event says it's {args.Value}. Pin says it's {gp_input.State}.");
                };

                // BUG BUG BUG - we don't pass this currently
                Assert.Equal(0, meadow_interrupt_count, "We got an unsolicted interrupt from the MCP Input Creation");

                // TODO: add loopbacks - for now it's a manual test.  Just pull GP low and then back high.
                for (int i = 0; i < 30; i++)
                {
                    Output.WriteLineIf(ShowDebug, $"tick");
                    Thread.Sleep(1000);
                }
            }
        }
    }
}