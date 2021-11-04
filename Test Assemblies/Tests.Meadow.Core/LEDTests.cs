using System.Threading;
using Meadow.Devices;
using Meadow.Hardware;
using Munit;

namespace MeadowLibary
{

    public class LEDTests
    {
        public F7MicroBase Device { get; set; }

        [Fact]
        public void LedTestFunction()
        {
            // there's no great way to automate if the LEDs work, we'll just test that we can create and change state and assume that if all that works (i.e. doesn't throw) we're good

            // We don't have access to the concrete Device instance, so the specific Pins aren't directly available.
            // You must request them by string, which can be either the name or ID.
            // For most, it's simple - "D01" or "A02" for instance works.  The onboard LED names are less friendly.
            var green = Device.GetPin("OnboardLedGreen");
            var red = Device.GetPin("OnboardLedRed");
            var blue = Device.GetPin("OnboardLedBlue");

            Assert.NotNull(green);
            Assert.NotNull(red);
            Assert.NotNull(blue);

            var leds = new IDigitalOutputPort[3];
            leds[0] = Device.CreateDigitalOutputPort(green);
            leds[1] = Device.CreateDigitalOutputPort(red);
            leds[2] = Device.CreateDigitalOutputPort(blue);

            for(int i = 0; i < leds.Length; i++)
            {
                leds[i].State = true;
                Thread.Sleep(500);
                leds[i].State = false;
            }
        }

        [Fact]
        public void LedTestDuplicateInstance()
        {
            var green = Device.GetPin("OnboardLedGreen");

            Assert.NotNull(green);

            var leds = new IDigitalOutputPort[2];

            // this should be illegal and throw
            Assert.Throws<PortInUseException>(() =>
            {
                leds[0] = Device.CreateDigitalOutputPort(green);
                leds[1] = Device.CreateDigitalOutputPort(green);
            });

        }
    }
}