using Meadow.Hardware;
using Munit;
using System;
using System.Linq;

namespace MeadowLibary
{
    public partial class InterruptTests
    {
        [Fact]
        public void InterruptGroup0Test()
        {
            // group 0: A04, D06
            InterruptGroupTest("A04", "D06");
        }

        private void InterruptGroupTest(params string[] group)
        {
            var firstAnalog = 0;
            var lastAnalog = 5;
            var firstDigital = 2; // skip 0 & 1 because they're used for the serial port
            var lastDigital = 15; // skip 0 & 1 because they're used for the serial port

            var pinA = Device.GetPin(group[0]);

            using (var portA = Device.CreateDigitalInputPort(pinA, resistorMode: ResistorMode.InternalPullDown, interruptMode: InterruptMode.EdgeRising))
            {
                for(int i = firstAnalog; i <= lastAnalog; i++)
                {
                    var name = $"A{i:00}";
                    // skip self
                    if (name == group[0]) continue;

                    var testPin = Device.GetPin(name);
                    if (group.Contains(name))
                    {
                        Assert.Throws<InterruptGroupInUseException>(() =>
                        {
                            using (var portB = Device.CreateDigitalInputPort(testPin, resistorMode: ResistorMode.InternalPullDown, interruptMode: InterruptMode.EdgeRising))
                            {
                            }
                        });
                    }
                    else
                    {
                        using (var portB = Device.CreateDigitalInputPort(testPin, resistorMode: ResistorMode.InternalPullDown, interruptMode: InterruptMode.EdgeRising))
                        {
                            Assert.NotNull(portB);
                        }
                    }
                }

                for (int i = firstDigital; i <= lastDigital; i++)
                {
                    var name = $"D{i:00}";
                    // skip self
                    if (name == group[0]) continue;

                    var testPin = Device.GetPin(name);
                    if (group.Contains(name))
                    {
                        Assert.Throws<InterruptGroupInUseException>(() =>
                        {
                            using (var portB = Device.CreateDigitalInputPort(testPin, resistorMode: ResistorMode.InternalPullDown, interruptMode: InterruptMode.EdgeRising))
                            {
                            }
                        });
                    }
                    else
                    {
                        using (var portB = Device.CreateDigitalInputPort(testPin, resistorMode: ResistorMode.InternalPullDown, interruptMode: InterruptMode.EdgeRising))
                        {
                            Assert.NotNull(portB);
                        }
                    }
                }
            }
        }
    }
}