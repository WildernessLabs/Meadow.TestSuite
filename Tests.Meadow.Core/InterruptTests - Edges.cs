using Meadow.Hardware;
using Meadow.TestSuite;
using Munit;
using System;
using System.EnterpriseServices;
using System.Linq;
using System.Threading;

namespace MeadowLibary
{
    public partial class InterruptTests
    {
        public bool ShowDebug { get; set; } = false;
        public IIODevice Device { get; set; }

        // DEV NOTE
        // with Meadow b4.2 (likely anything pre-AOT) the first interrupt handler is way slow, so we need to account for that during testing
        private static bool VeryFirstInterrupt { get; set; } = true;

        // DEV NOTE
        // Dec 3, 20 meadow b4.x
        // any faster than this and tests start failing
        // not sure if it's electrical (the transition just not happening fast) or a code thing.
        private const int DefaultTimeoutMs = 40;
        private const int SettleTimeoutMs = 20;

        [Fact]
        public void InterruptA0_A1()
        {
            FullInterruptTests("A00", "A01");
        }

        [Fact]
        public void InterruptA2_A3()
        {
            FullInterruptTests("A02", "A03");
        }

        [Fact]
        public void InterruptA4_A5()
        {
            FullInterruptTests("A04", "A05");
        }

        [Fact]
        public void InterruptMOSI_MISO()
        {
            FullInterruptTests("MOSI", "MISO");
        }

        [Fact]
        public void Interrupt3_4()
        {
            FullInterruptTests("D03", "D04");
        }

        [Fact]
        public void Interrupt5_6()
        {
            FullInterruptTests("D05", "D06");
        }

        [Fact]
        public void Interrupt7_8()
        {
            FullInterruptTests("D07", "D08");
        }

        [Fact]
        public void Interrupt9_10()
        {
            FullInterruptTests("D09", "D10");
        }

        [Fact]
        public void Interrupt11_12()
        {
            FullInterruptTests("D11", "D12");
        }

        [Fact]
        public void Interrupt13_14()
        {
            FullInterruptTests("D13", "D14");
        }

        [Fact]
        public void Interrupt15_2()
        {
            FullInterruptTests("D15", "D02");
        }

        private void FullInterruptTests(string pinA, string pinB, int iterations = 3)
        {
            InterruptTestRising(pinA, pinB, iterations);
            InterruptTestFalling(pinA, pinB, iterations);
            InterruptTestBidirectional(pinA, pinB, iterations);

            InterruptTestRising(pinB, pinA, iterations);
            InterruptTestFalling(pinB, pinA, iterations);
            InterruptTestBidirectional(pinB, pinA, iterations);
        }

        private void InterruptTestRising(string source, string sink, int edgeCount, int sleepTimeMs = DefaultTimeoutMs)
        {
            Output.WriteLineIf(ShowDebug, "+InterruptTestRising");

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

                using (var input = Device.CreateDigitalInputPort(
                    pinB, 
                    resistorMode: ResistorMode.PullDown, 
                    interruptMode: InterruptMode.EdgeRising,
                    debounceDuration: 10,
                    glitchDuration: 10
                    ))
                {
                    var interruptDetected = false;
                    var interruptState = false;
                    var count = 0;

                    input.Changed += (s, e) =>
                    {
                        Output.WriteLineIf(ShowDebug, $"interrupt {count}");
                        interruptDetected = true;
                        interruptState = e.Value;
                        count++;
                        VeryFirstInterrupt = false;
                    };

                    // give time for the interrupt to connect
                    Thread.Sleep(sleepTimeMs);

                    for (int i = 0; i < edgeCount; i++)
                    {
                        // state checks
                        output.State = true;

                        // give time for the interrupt to fire
                        if (VeryFirstInterrupt)
                        {
                            // just Meadow being Meadow without AoT - very first interrupt is sloooooooooowwwwww
                            Thread.Sleep(1000 + sleepTimeMs);
                        }
                        else
                        {
                            // this is instead of a single sleep just as a test to see if waking makes a timing difference for the slowness (i.e. interrupt thread contention)
                            for (var t = 0; t < sleepTimeMs; t += 10)
                            {
                                Thread.Sleep(10);
                            }
                        }

                        output.State = false;
                        for (var t = 0; t < SettleTimeoutMs; t += 10)
                        {
                            Thread.Sleep(10);
                        }

                        Output.WriteLineIf(ShowDebug, $"check {i + 1}");
                        Assert.True(interruptDetected, "Rising interrupt not detected");
                        Assert.True(interruptState, "Interrupt event state not correct");
                        Assert.Equal(i + 1, count, $"Unexpected interrupt count of {count}");
                    }
                }
            }
        }

        private void InterruptTestFalling(string source, string sink, int edgeCount, int sleepTimeMs = DefaultTimeoutMs)
        {
            Output.WriteLineIf(ShowDebug, "+InterruptTestFalling");

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

                using (var input = Device.CreateDigitalInputPort(
                    pinB, 
                    resistorMode: ResistorMode.PullUp, 
                    interruptMode: InterruptMode.EdgeFalling,
                    debounceDuration: 10,
                    glitchDuration: 10
                    ))
                {
                    var interruptDetected = false;
                    var interruptState = false;
                    var count = 0;

                    input.Changed += (s, e) =>
                    {
                        interruptDetected = true;
                        interruptState = e.Value;
                        count++;
                        VeryFirstInterrupt = false;
                    };

                    // give time for the interrupt to connect
                    Thread.Sleep(sleepTimeMs);

                    for (int i = 0; i < edgeCount; i++)
                    {
                        // state checks
                        output.State = false;

                        // give time for the interrupt to fire
                        if (VeryFirstInterrupt)
                        {
                            // just Meadow being Meadow without AoT - very first interrupt is sloooooooooowwwwww
                            Thread.Sleep(1000 + sleepTimeMs);
                        }
                        else
                        {
                            Thread.Sleep(sleepTimeMs);
                        }

                        output.State = true;
                        Thread.Sleep(SettleTimeoutMs);

                        Assert.True(interruptDetected, "Falling interrupt not detected");
                        Assert.False(interruptState, "Interrupt event state not correct");
                        Assert.Equal(i + 1, count, $"Unexpected interrupt count of {count}");
                    }
                }
            }
        }

        private void InterruptTestBidirectional(string source, string sink, int cycleCount, int sleepTimeMs = DefaultTimeoutMs)
        {
            Output.WriteLineIf(ShowDebug, "+InterruptTestBidirectional");

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
                        interruptState[count % 2] = e.Value;
                        count++;
                        VeryFirstInterrupt = false;
                    };

                    // give time for the interrupt to connect
                    Thread.Sleep(sleepTimeMs);

                    for (int i = 0; i < cycleCount; i++)
                    {
                        // state checks
                        output.State = true;
                        // give time for the interrupt to fire
                        if (VeryFirstInterrupt)
                        {
                            // just Meadow being Meadow without AoT - very first interrupt is sloooooooooowwwwww
                            Thread.Sleep(1000 + sleepTimeMs);
                        }
                        else
                        {
                            Thread.Sleep(sleepTimeMs);
                        }

                        output.State = false;
                        // give time for the interrupt to fire
                        Thread.Sleep(sleepTimeMs);

                        Assert.Equal((i + 1) * 2, count, $"Unexpected interrupt count of {count}");
                        Assert.True(interruptState[0], "First interrupt event state not correct");
                        Assert.False(interruptState[1], "Second interrupt event state not correct");
                    }
                }
            }
        }
    }
}