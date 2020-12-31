using Meadow.Hardware;
using Meadow.TestSuite;
using Munit;

namespace MeadowLibary
{
    public partial class GpioTests
    {
        public bool ShowDebug { get; set; } = false;
        public IIODevice Device { get; set; }

        [Fact]
        public void PortInUseValidations()
        {
            var pins = new string[]
            {
                "A00", "A01", "A02", "A03", "A04", "A05", "A06",
                "SCK", "MOSI", "MISO",
                "D02", "D03", "D04", "D05", "D06", "D07", "D08",
                "D09", "D10", "D11", "D12", "D13", "D14", "D15"
            };

            foreach(var p in pins)
            {
                PortInUseValidation(p);
            }
        }

        private void PortInUseValidation(string pin)
        {
            Assert.Throws<PortInUseException>(() =>
            {
                var p = Device.GetPin(pin);

                using (var a = Device.CreateDigitalOutputPort(p))
                using (var b = Device.CreateDigitalOutputPort(p))
                {
                    // nop - just ensuring Dispose gets called
                }
            }, $"Pin {pin} was allowed duplication");

            Assert.Throws<PortInUseException>(() =>
            {
                var p = Device.GetPin(pin);
                using (var a = Device.CreateDigitalInputPort(p))
                using (var b = Device.CreateDigitalInputPort(p))
                {
                    // nop - just ensuring Dispose gets called
                }
            }, $"Pin {pin} was allowed duplication");

            Assert.Throws<PortInUseException>(() =>
            {
                var p = Device.GetPin(pin);
                using (var a = Device.CreateDigitalOutputPort(p))
                using (var b = Device.CreateDigitalInputPort(p))
                {
                    // nop - just ensuring Dispose gets called
                }
            }, $"Pin {pin} was allowed duplication");
        }

        [Fact]
        public void LoopbackA0_A1()
        {
            LoopbackPins("A00", "A01");
        }

        [Fact]
        public void LoopbackA2_A3()
        {
            LoopbackPins("A02", "A03");
        }

        [Fact]
        public void LoopbackA4_A5()
        {
            LoopbackPins("A04", "A05");
        }

        [Fact]
        public void LoopbackMOSI_MISO()
        {
            LoopbackPins("MOSI", "MISO");
        }

        [Fact]
        public void Loopback3_4()
        {
            LoopbackPins("D03", "D04");
        }

        [Fact]
        public void Loopback5_6()
        {
            LoopbackPins("D05", "D06");
        }

        [Fact]
        public void Loopback7_8()
        {
            LoopbackPins("D07", "D08");
        }

        [Fact]
        public void Loopback9_10()
        {
            LoopbackPins("D09", "D10");
        }

        [Fact]
        public void Loopback11_12()
        {
            LoopbackPins("D11", "D12");
        }

        [Fact]
        public void Loopback13_14()
        {
            LoopbackPins("D13", "D14");
        }

        [Fact]
        public void Loopback15_2()
        {
            LoopbackPins("D15", "D02");
        }

        private void LoopbackPins(string a, string b)
        {

            // We don't have access to the concrete Device instance, so the specific Pins aren't directly available.
            // You must request them by string, which can be either the name or ID.
            // For most, it's simple - "D01" or "A02" for instance works.  The onboard LED names are less friendly.
            Output.WriteLineIf(ShowDebug, $"Getting Pins '{a}' and '{b}'...");
            var pinA = Device.GetPin(a);
            var pinB = Device.GetPin(b);

            Assert.NotNull(pinA);
            Assert.NotNull(pinB);

            Output.WriteLineIf(ShowDebug, $"Creating Ports for '{a}'->'{b}'...");
            using (var output = Device.CreateDigitalOutputPort(pinA))
            using (var input = Device.CreateDigitalInputPort(pinB, resistorMode: ResistorMode.PullDown))
            {
                Output.WriteLineIf(ShowDebug, $"Checking initial state of '{b}'...");
                Assert.False(input.State, $"{b} Expected to be pulled low");

                // state checks
                Output.WriteLineIf(ShowDebug, $"Checking '{a}' high...");
                output.State = true;
                Assert.True(output.State, $"{a} Expected to be asserted high");
                Assert.True(input.State, $"{b} Expected to be driven high");

                Output.WriteLineIf(ShowDebug, $"Checking '{a}' low...");
                output.State = false;
                Assert.False(output.State, $"{a} Expected to be asserted low");
                Assert.False(input.State, $"{b} Expected to be driven low");
            }

            Output.WriteLineIf(ShowDebug, $"Creating Ports for '{b}'->'{a}'...");
            using (var output = Device.CreateDigitalOutputPort(pinB))
            using (var input = Device.CreateDigitalInputPort(pinA, resistorMode: ResistorMode.PullDown))
            {
                Output.WriteLineIf(ShowDebug, $"Checking initial state of '{a}'...");
                Assert.False(input.State, $"{a} Expected to be pulled low");

                // state checks
                Output.WriteLineIf(ShowDebug, $"Checking '{b}' high...");
                output.State = true;
                Assert.True(output.State, $"{b} Expected to be asserted high");
                Assert.True(input.State, $"{a} Expected to be driven high");

                Output.WriteLineIf(ShowDebug, $"Checking '{b}' low...");
                output.State = false;
                Assert.False(output.State, $"{b} Expected to be asserted low");
                Assert.False(input.State, $"{a} Expected to be driven low");
            }
        }
    }
}