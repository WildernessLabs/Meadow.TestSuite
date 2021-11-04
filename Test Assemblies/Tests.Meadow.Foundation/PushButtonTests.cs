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
        public bool ShowDebug { get; set; } = false;

        // DEV NOTE
        // with Meadow b4.2 (likely anything pre-AOT) the first interrupt handler is way slow, so we need to account for that during testing
        private static bool VeryFirstInterrupt { get; set; } = true;

        public Meadow.Devices.F7MicroBase Device { get; set; }

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
                ResistorMode.InternalPullUp,
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
                    button.LongClicked += delegate { };
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
                ResistorMode.InternalPullUp,
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
                    button.LongClicked += delegate { };
                });
            }
        }

        [Fact]
        public void PressStartedTest()
        {
            var startsDetected = 0;

            var buttonPin = Device.GetPin(ButtonPinIdentifier);
            var drivePin = Device.GetPin(DrivePinIdentifier);

            Output.WriteLineIf(ShowDebug, $"Creating output on {drivePin.Key}");

            using (var output = Device.CreateDigitalOutputPort(drivePin))
            {
                output.State = true;

                Output.WriteLineIf(ShowDebug, $"Creating input on {buttonPin.Key}");
                using (var buttonInput = Device.CreateDigitalInputPort(
                    buttonPin,
                    InterruptMode.EdgeFalling,
                    ResistorMode.InternalPullUp,
                    20,
                    20))
                {
                    Output.WriteLineIf(ShowDebug, $"Creating PushButton");
                    var button = new PushButton(buttonInput);

                    button.PressStarted += (s, e) =>
                    {
                        Output.WriteLineIf(ShowDebug, $"Press Started");
                        startsDetected++;

                        VeryFirstInterrupt = false;
                    };

                    for (var i = 0; i < ClicksToTest; i++)
                    {
                        Output.WriteLineIf(ShowDebug, $"Click {i + 1}");

                        // "click" the button n times. and make sure we got that many clicks
                        output.State = false;

                        // wait a little while
                        Thread.Sleep(250 + (VeryFirstInterrupt ? 1000 : 0));

                        output.State = true;

                        // wait a little while
                        Thread.Sleep(250);
                    }

                    Output.WriteLineIf(ShowDebug, $"Dispose input");
                }

                Output.WriteLineIf(ShowDebug, $"Dispose output");
            }

            Assert.Equal(ClicksToTest, startsDetected);
        }

        [Fact]
        public void ClickTest()
        {
            var clicksDetected = 0;

            var buttonPin = Device.GetPin(ButtonPinIdentifier);
            var drivePin = Device.GetPin(DrivePinIdentifier);

            Output.WriteLineIf(ShowDebug, $"Creating output on {drivePin.Key}");

            using (var output = Device.CreateDigitalOutputPort(drivePin))
            {
                output.State = true;

                Output.WriteLineIf(ShowDebug, $"Creating input on {buttonPin.Key}");
                using (var buttonInput = Device.CreateDigitalInputPort(
                    buttonPin,
                    InterruptMode.EdgeBoth,
                    ResistorMode.InternalPullUp,
                    20,
                    20))
                {
                    Output.WriteLineIf(ShowDebug, $"Creating PushButton");
                    var button = new PushButton(buttonInput);

                    button.Clicked += (s, e) =>
                    {
                        Output.WriteLineIf(ShowDebug, $"Clicked");
                        clicksDetected++;

                        VeryFirstInterrupt = false;
                    };

                    for (var i = 0; i < ClicksToTest; i++)
                    {
                        Output.WriteLineIf(ShowDebug, $"Click {i + 1}");

                        // "click" the button n times. and make sure we got that many clicks
                        output.State = false;

                        // wait a little while
                        Thread.Sleep(250 + (VeryFirstInterrupt ? 1000 : 0));

                        output.State = true;

                        // wait a little while
                        Thread.Sleep(250);
                    }

                    Output.WriteLineIf(ShowDebug, $"Dispose input");
                }

                Output.WriteLineIf(ShowDebug, $"Dispose output");
            }

            Assert.Equal(ClicksToTest, clicksDetected);
        }

        [Fact]
        public void PressEndedTest()
        {
            var clicksDetected = 0;

            var buttonPin = Device.GetPin(ButtonPinIdentifier);
            var drivePin = Device.GetPin(DrivePinIdentifier);

            Output.WriteLineIf(ShowDebug, $"Creating output on {drivePin.Key}");

            using (var output = Device.CreateDigitalOutputPort(drivePin))
            {
                output.State = true;

                Output.WriteLineIf(ShowDebug, $"Creating input on {buttonPin.Key}");
                using (var buttonInput = Device.CreateDigitalInputPort(
                    buttonPin,
                    InterruptMode.EdgeBoth,
                    ResistorMode.InternalPullUp,
                    20,
                    20))
                {
                    Output.WriteLineIf(ShowDebug, $"Creating PushButton");
                    var button = new PushButton(buttonInput);

                    button.PressEnded += (s, e) =>
                    {
                        Output.WriteLineIf(ShowDebug, $"Ended");
                        clicksDetected++;

                        VeryFirstInterrupt = false;
                    };

                    for (var i = 0; i < ClicksToTest; i++)
                    {
                        Output.WriteLineIf(ShowDebug, $"Click {i + 1}");

                        // "click" the button n times. and make sure we got that many clicks
                        output.State = false;

                        // wait a little while
                        Thread.Sleep(250 + (VeryFirstInterrupt ? 1000 : 0));

                        output.State = true;

                        // wait a little while
                        Thread.Sleep(250);
                    }

                    Output.WriteLineIf(ShowDebug, $"Dispose input");
                }

                Output.WriteLineIf(ShowDebug, $"Dispose output");
            }

            Assert.Equal(ClicksToTest, clicksDetected);
        }
    }
}