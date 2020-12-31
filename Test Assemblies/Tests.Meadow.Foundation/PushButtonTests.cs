using Meadow.Foundation.Sensors.Buttons;
using Meadow.Hardware;
using Meadow.TestSuite;
using Munit;
using System;
using System.Threading;

namespace Foundation
{
    public partial class PushButtonTests
    {
        // DEV NOTE
        // with Meadow b4.2 (likely anything pre-AOT) the first interrupt handler is way slow, so we need to account for that during testing
        private static bool VeryFirstInterrupt { get; set; } = true;

        public IIODevice Device { get; set; }

        private const string DrivePinIdentifier = "D04";
        private const string ButtonPinIdentifier = "D03";
        private const int ClicksToTest = 3;

        [Fact]
        public void NoInterruptsCheck()
        {
            var buttonPin = Device.GetPin(ButtonPinIdentifier);

            using (var buttonInput = Device.CreateDigitalInputPort(
                buttonPin,
                InterruptMode.None,
                ResistorMode.PullUp,
                20,
                20))
            {
                // no interrupt edge should puke
                var button = new PushButton(buttonInput);

                Assert.Throws<Exception>(() =>
                {
                    button.PressStarted += delegate { };
                });

                Assert.Throws<Exception>(() =>
                {
                    button.PressEnded += delegate { };
                });

                Assert.Throws<Exception>(() =>
                {
                    button.Clicked += delegate { };
                });

                Assert.Throws<Exception>(() =>
                {
                    button.LongPressClicked += delegate { };
                });
            }
        }

        [Fact]
        public void OneEdgeInterruptCheck()
        {
            var buttonPin = Device.GetPin(ButtonPinIdentifier);

            using (var buttonInput = Device.CreateDigitalInputPort(
                buttonPin,
                InterruptMode.EdgeFalling,
                ResistorMode.PullUp,
                20,
                20))
            {
                // no interrupt edge should puke
                var button = new PushButton(buttonInput);

                Assert.Throws<Exception>(() =>
                {
                    button.PressEnded += delegate { };
                });

                Assert.Throws<Exception>(() =>
                {
                    button.Clicked += delegate { };
                });

                Assert.Throws<Exception>(() =>
                {
                    button.LongPressClicked += delegate { };
                });
            }
        }

        [Fact]
        public void PressStartedTest_PortConstructor()
        {
            var startsDetected = 0;

            var buttonPin = Device.GetPin(ButtonPinIdentifier);
            var drivePin = Device.GetPin(DrivePinIdentifier);

            using (var output = Device.CreateDigitalOutputPort(drivePin))
            {
                output.State = true;

                using (var buttonInput = Device.CreateDigitalInputPort(
                    buttonPin,
                    InterruptMode.EdgeFalling,
                    ResistorMode.PullUp,
                    20,
                    20))
                {
                    var button = new PushButton(buttonInput);

                    button.PressStarted += (s, e) =>
                    {
                        startsDetected++;

                        VeryFirstInterrupt = false;
                    };

                    for (var i = 0; i < ClicksToTest; i++)
                    {
                        // "click" the button n times. and make sure we got that many clicks
                        output.State = false;

                        // wait a little while
                        Thread.Sleep(250 + (VeryFirstInterrupt ? 1000 : 0));

                        output.State = true;

                        // wait a little while
                        Thread.Sleep(250);
                    }
                }
            }

            Assert.Equal(ClicksToTest, startsDetected);
        }

        [Fact]
        public void ClickTest_PortConstructor()
        {
            var clicksDetected = 0;

            var buttonPin = Device.GetPin(ButtonPinIdentifier);
            var drivePin = Device.GetPin(DrivePinIdentifier);

            using (var output = Device.CreateDigitalOutputPort(drivePin))
            {
                output.State = true;

                using (var buttonInput = Device.CreateDigitalInputPort(
                    buttonPin,
                    InterruptMode.EdgeBoth,
                    ResistorMode.PullUp,
                    20,
                    20))
                {
                    var button = new PushButton(buttonInput);

                    button.Clicked += (s, e) =>
                    {
                        clicksDetected++;

                        VeryFirstInterrupt = false;
                    };

                    for (var i = 0; i < ClicksToTest; i++)
                    {
                        // "click" the button n times. and make sure we got that many clicks
                        output.State = false;

                        // wait a little while
                        Thread.Sleep(250 + (VeryFirstInterrupt ? 1000 : 0));

                        output.State = true;

                        // wait a little while
                        Thread.Sleep(250);
                    }
                }
            }

            Assert.Equal(ClicksToTest, clicksDetected);
        }

        [Fact]
        public void ClickTest_DeviceAndPinConstructor()
        {
            var clicksDetected = 0;

            var buttonPin = Device.GetPin(ButtonPinIdentifier);
            var drivePin = Device.GetPin(DrivePinIdentifier);

            using (var output = Device.CreateDigitalOutputPort(drivePin))
            {
                output.State = true;

                using (var button = new PushButton(Device, buttonPin, ResistorMode.PullUp, 20))
                {

                    button.Clicked += (s, e) =>
                    {
                        clicksDetected++;

                        VeryFirstInterrupt = false;
                    };

                    for (var i = 0; i < ClicksToTest; i++)
                    {
                        // "click" the button n times. and make sure we got that many clicks
                        output.State = false;

                        // wait a little while
                        Thread.Sleep(250 + (VeryFirstInterrupt ? 1000 : 0));

                        output.State = true;

                        // wait a little while
                        Thread.Sleep(250);
                    }
                }
            }

            Assert.Equal(ClicksToTest, clicksDetected);
        }

        [Fact]
        public void PressEndedTest_PortConstructor()
        {
            var clicksDetected = 0;

            var buttonPin = Device.GetPin(ButtonPinIdentifier);
            var drivePin = Device.GetPin(DrivePinIdentifier);

            using (var output = Device.CreateDigitalOutputPort(drivePin))
            {
                output.State = true;

                using (var buttonInput = Device.CreateDigitalInputPort(
                    buttonPin,
                    InterruptMode.EdgeBoth,
                    ResistorMode.PullUp,
                    20,
                    20))
                {
                    var button = new PushButton(buttonInput);

                    button.PressEnded += (s, e) =>
                    {
                        clicksDetected++;

                        VeryFirstInterrupt = false;
                    };

                    for (var i = 0; i < ClicksToTest; i++)
                    {
                        // "click" the button n times. and make sure we got that many clicks
                        output.State = false;

                        // wait a little while
                        Thread.Sleep(250 + (VeryFirstInterrupt ? 1000 : 0));

                        output.State = true;

                        // wait a little while
                        Thread.Sleep(250);
                    }
                }
            }

            Assert.Equal(ClicksToTest, clicksDetected);
        }

        [Fact]
        public void LongPressTest_PortConstructor()
        {
            var longPressesDetected = 0;

            var buttonPin = Device.GetPin(ButtonPinIdentifier);
            var drivePin = Device.GetPin(DrivePinIdentifier);

            using (var output = Device.CreateDigitalOutputPort(drivePin))
            {
                output.State = true;

                using (var buttonInput = Device.CreateDigitalInputPort(
                    buttonPin,
                    InterruptMode.EdgeBoth,
                    ResistorMode.PullUp,
                    20,
                    20))
                {
                    var button = new PushButton(buttonInput);
                    button.LongPressThreshold = TimeSpan.FromMilliseconds(500);

                    button.LongPressClicked += (s, e) =>
                    {
                        longPressesDetected++;

                        VeryFirstInterrupt = false;
                    };

                    // this throws things off, so let's just sink it if it's true
                    if(VeryFirstInterrupt)
                    {
                        // "click" the button n times. and make sure we got that many clicks
                        output.State = false;

                        // wait a little while
                        Thread.Sleep(1000);

                        output.State = true;

                        longPressesDetected = 0;
                    }

                    // first, let's do a few short presses and make sure we *dont" get events
                    for (var i = 0; i < ClicksToTest; i++)
                    {
                        // "click" the button n times. and make sure we got that many clicks
                        output.State = false;

                        // wait less than the threshold
                        Thread.Sleep(button.LongPressThreshold.Subtract(TimeSpan.FromMilliseconds(100)));

                        output.State = true;

                        // wait a while before we click again
                        Thread.Sleep(250);
                    }

                    Assert.Equal(0, longPressesDetected, "Short presses created long press events");

                    // now do a few long presses and make sure we *do" get events
                    for (var i = 0; i < ClicksToTest; i++)
                    {
                        // "click" the button n times. and make sure we got that many clicks
                        output.State = false;

                        // wait more than the threshold
                        Thread.Sleep(button.LongPressThreshold.Add(TimeSpan.FromMilliseconds(100)));

                        output.State = true;

                        // wait a while before we click again
                        Thread.Sleep(250);
                    }

                    Assert.Equal(ClicksToTest, longPressesDetected);
                }
            }
        }
    }
}