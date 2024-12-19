using FeatherF7Test.Hardware;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Displays;

namespace FeatherF7Test.Hardware
{
    internal class OLEDBoardHardware : IOLEDBoardHardware
    {
        public IGraphicsDisplay Display { get; private set; }

        public Led Led1 { get; private set; }

        public Led Led2 { get; private set; }

        public Led Led3 { get; private set; }

        public Led Led4 { get; private set; }

        public void Initialize(F7FeatherBase device)
        {
            Display = new Ssd1306
            (
                i2cBus: device.CreateI2cBus(Meadow.Hardware.I2cBusSpeed.FastPlus),
                address: 60,
                displayType: Ssd1306.DisplayType.OLED128x32
            );

            Led1 = new Led(device.CreateDigitalOutputPort(device.Pins.D01))
            {
                IsOn = false
            };
            Led2 = new Led(device.CreateDigitalOutputPort(device.Pins.D02))
            {
                IsOn = false
            };
            Led3 = new Led(device.CreateDigitalOutputPort(device.Pins.D03))
            {
                IsOn = false
            };
            Led4 = new Led(device.CreateDigitalOutputPort(device.Pins.D04))
            {
                IsOn = false
            };
        }
    }
}