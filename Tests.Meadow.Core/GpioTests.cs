using System;
using System.Threading;
using Meadow.Hardware;
using Munit;

namespace MeadowLibary
{
    public partial class GpioTests
    {
        public IIODevice Device { get; set; }

        [Fact]
        public void Loopback5_6()
        {
            // We don't have access to the concrete Device instance, so the specific Pins aren't directly available.
            // You must request them by string, which can be either the name or ID.
            // For most, it's simple - "D01" or "A02" for instance works.  The onboard LED names are less friendly.
            var d05 = Device.GetPin("D05");
            var d06 = Device.GetPin("D06");

            Assert.NotNull(d05);
            Assert.NotNull(d06);

            using (var output = Device.CreateDigitalOutputPort(d05))
            using (var input = Device.CreateDigitalInputPort(d06, resistorMode: ResistorMode.PullDown))
            {
                Assert.False(input.State, "D06 Expected to be pulled low");

                // state checks
                output.State = true;
                Assert.True(output.State, "D05 Expected to be asserted high");
                Assert.True(input.State, "D06 Expected to be driven high");
                output.State = false;
                Assert.False(output.State, "D05 Expected to be asserted low");
                Assert.False(input.State, "D06 Expected to be driven low");
            }

            using (var output = Device.CreateDigitalOutputPort(d06))
            using (var input = Device.CreateDigitalInputPort(d05, resistorMode: ResistorMode.PullDown))
            {
                Assert.False(input.State, "D05 Expected to be pulled low");

                // state checks
                output.State = true;
                Assert.True(output.State, "D06 Expected to be asserted high");
                Assert.True(input.State, "D05 Expected to be driven high");
                output.State = false;
                Assert.False(output.State, "D06 Expected to be asserted low");
                Assert.False(input.State, "D05 Expected to be driven low");
            }
        }
    }
}