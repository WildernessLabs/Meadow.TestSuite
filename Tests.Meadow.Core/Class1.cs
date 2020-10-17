using System;
using System.Threading;
using Meadow.Hardware;
using Meadow.TestSuite;
using Munit;

namespace MeadowLibary
{
    public class LEDTest
    {
        [Fact]
        public void FactMethod()
        {

        }
        [Fact]
        public void FactMethod2()
        {

        }
    }

    public class GpioTests
    {
        public IIODevice Device { get; set; }

        [Fact]
        public void LedTest()
        {
            Console.WriteLine("Starting LedTest");

            // We don't have access to the concrete Device instance, so the specific Pins aren't directly available.
            // You must request them by string, which can be either the name or ID.
            // For most, it's simple - "D01" or "A02" for instance works.  The onboard LED names are less friendly.
            var green = Device.GetPin("OnboardLedGreen");

            Assert.NotNull(green);

            var led = Device.CreateDigitalOutputPort(green);
            Console.WriteLine("On");
            led.State = true;
            Thread.Sleep(1000);
            Console.WriteLine("Off");
            led.State = false;

            Console.WriteLine("LedTest Complete");
        }
    }
}