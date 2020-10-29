using Meadow.Hardware;
using Munit;
using System;
using System.Linq;
using System.Threading;

namespace MeadowLibary
{
    public partial class InterruptTests
    {
        public IIODevice Device { get; set; }

        [Fact]
        public void Interrupt3_4()
        {
            InterruptTestRising("D03", "D04", 3, 5000);
            /*
            InterruptTestFalling("D03", "D04");
            InterruptTestBidirectional("D03", "D04");

            InterruptTestRising("D04", "D03", 3);
            InterruptTestFalling("D04", "D03");
            InterruptTestBidirectional("D04", "D03");
            */
        }

        private void InterruptTestRising(string source, string sink, int edgeCount, int sleepTimeMs = 200)
        {
            Console.WriteLine("+InterruptTestRising");

            // We don't have access to the concrete Device instance, so the specific Pins aren't directly available.
            // You must request them by string, which can be either the name or ID.
            // For most, it's simple - "D01" or "A02" for instance works.  The onboard LED names are less friendly.
            var pinA = Device.GetPin(source);
            var pinB = Device.GetPin(sink);

            Assert.NotNull(pinA);
            Assert.NotNull(pinB);

            using (var output = Device.CreateDigitalOutputPort(pinA))
            {
                output.State = false;

                using (var input = Device.CreateDigitalInputPort(pinB, resistorMode: ResistorMode.PullDown, interruptMode: InterruptMode.EdgeRising))
                {
                    var interruptDetected = false;
                    var interruptState = false;
                    var count = 0;

                    input.Changed += (s, e) =>
                    {
                        Console.WriteLine("Interrupt event");

                        interruptDetected = true;
                        interruptState = e.Value;
                        count++;
                    };

                    for (int i = 0; i < edgeCount; i++)
                    {
                        // give time for the interrupt to connect
                        Thread.Sleep(sleepTimeMs);

                        // state checks
                        output.State = true;

                        // give time for the interrupt to fire
                        Thread.Sleep(sleepTimeMs);

                        Assert.True(output.State, $"{source} Expected to be asserted high");
                        Assert.True(input.State, $"{sink} Expected to be driven high");

                        Assert.True(interruptDetected, "Rising interrupt not detected");
                        Assert.True(interruptState, "Interrupt event state not correct");

                        output.State = false;
                    }

                    Console.WriteLine("Checking count");
                    Assert.Equal(edgeCount, count, $"Unexpected interrupt count of {count}");

                }
            }
        }

        private void InterruptTestFalling(string source, string sink)
        {
            Console.WriteLine("+InterruptTestFalling");

            // We don't have access to the concrete Device instance, so the specific Pins aren't directly available.
            // You must request them by string, which can be either the name or ID.
            // For most, it's simple - "D01" or "A02" for instance works.  The onboard LED names are less friendly.
            var pinA = Device.GetPin(source);
            var pinB = Device.GetPin(sink);

            Assert.NotNull(pinA);
            Assert.NotNull(pinB);

            using (var output = Device.CreateDigitalOutputPort(pinA))
            {
                output.State = true;

                using (var input = Device.CreateDigitalInputPort(pinB, resistorMode: ResistorMode.PullUp, interruptMode: InterruptMode.EdgeFalling))
                {
                    var interruptDetected = false;
                    var interruptState = false;
                    var count = 0;

                    input.Changed += (s, e) =>
                    {
                        interruptDetected = true;
                        interruptState = e.Value;
                        count++;
                    };

                    // state checks
                    output.State = false;
                    Assert.False(output.State, $"{source} Expected to be asserted low");
                    Assert.False(input.State, $"{sink} Expected to be driven low");
                    Assert.True(interruptDetected, "Falling interrupt not detected");
                    Assert.False(interruptState, "Interrupt event state not correct");
                    Assert.Equal(1, count, $"Unexpected interrupt count of {count}");

                }
            }
        }

        private void InterruptTestBidirectional(string source, string sink)
        {
            Console.WriteLine("+InterruptTestBidirectional");

            // We don't have access to the concrete Device instance, so the specific Pins aren't directly available.
            // You must request them by string, which can be either the name or ID.
            // For most, it's simple - "D01" or "A02" for instance works.  The onboard LED names are less friendly.
            var pinA = Device.GetPin(source);
            var pinB = Device.GetPin(sink);

            Assert.NotNull(pinA);
            Assert.NotNull(pinB);

            using (var output = Device.CreateDigitalOutputPort(pinA))
            {
                output.State = false;

                using (var input = Device.CreateDigitalInputPort(pinB, resistorMode: ResistorMode.PullDown, interruptMode: InterruptMode.EdgeBoth))
                {
                    var interruptState = new bool[2];
                    var count = 0;

                    input.Changed += (s, e) =>
                    {
                        if(count < 2)
                        {
                            interruptState[count] = e.Value;
                        }
                        count++;
                    };

                    // state checks
                    output.State = true;
                    output.State = false;

                    Assert.Equal(2, count, $"Unexpected interrupt count of {count}");
                    Assert.True(interruptState[0], "First interrupt event state not correct");
                    Assert.False(interruptState[1], "Second interrupt event state not correct");
                }
            }
        }
    }
}