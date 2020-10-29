﻿using System;
using System.Threading;
using Meadow.Hardware;
using Munit;

namespace MeadowLibary
{
    public partial class GpioTests
    {
        public IIODevice Device { get; set; }

        [Fact]
        public void PortInUseValidations()
        {
            var pins = new string[]
            {
                "D02", "D03","D04", "D05","D06", "D07", "D08",
                "D09", "D10", "D11","D12", "D13","D14", "D15"
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
                var a = Device.CreateDigitalOutputPort(p);
                var b = Device.CreateDigitalOutputPort(p);
            }, $"Pin {pin} was allowed duplication");

            Assert.Throws<PortInUseException>(() =>
            {
                var p = Device.GetPin(pin);
                var a = Device.CreateDigitalInputPort(p);
                var b = Device.CreateDigitalInputPort(p);
            }, $"Pin {pin} was allowed duplication");

            Assert.Throws<PortInUseException>(() =>
            {
                var p = Device.GetPin(pin);
                var a = Device.CreateDigitalOutputPort(p);
                var b = Device.CreateDigitalInputPort(p);
            }, $"Pin {pin} was allowed duplication");
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
            var pinA = Device.GetPin(a);
            var pinB = Device.GetPin(b);

            Assert.NotNull(pinA);
            Assert.NotNull(pinB);

            using (var output = Device.CreateDigitalOutputPort(pinA))
            using (var input = Device.CreateDigitalInputPort(pinB, resistorMode: ResistorMode.PullDown))
            {
                Assert.False(input.State, $"{b} Expected to be pulled low");

                // state checks
                output.State = true;
                Assert.True(output.State, $"{a} Expected to be asserted high");
                Assert.True(input.State, $"{b} Expected to be driven high");
                output.State = false;
                Assert.False(output.State, $"{a} Expected to be asserted low");
                Assert.False(input.State, $"{b} Expected to be driven low");
            }

            using (var output = Device.CreateDigitalOutputPort(pinB))
            using (var input = Device.CreateDigitalInputPort(pinA, resistorMode: ResistorMode.PullDown))
            {
                Assert.False(input.State, $"{a} Expected to be pulled low");

                // state checks
                output.State = true;
                Assert.True(output.State, $"{b} Expected to be asserted high");
                Assert.True(input.State, $"{a} Expected to be driven high");
                output.State = false;
                Assert.False(output.State, $"{b} Expected to be asserted low");
                Assert.False(input.State, $"{a} Expected to be driven low");
            }
        }
    }
}