using System;
using FeatherF7Test.Hardware;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Displays;

namespace FeatherF7Test.Hardware;
internal class OLEDBoardHardware : IOLEDBoardHardware
{
    /// <summary>
    /// SSD1306 OLED display to show progress.
    /// </summary>
    public IGraphicsDisplay Display { get; private set; }

    /// <summary>
    /// LEDS that can be used to indicate test progress / status.
    /// </summary>
    public Led[] Leds { get ; private set; }

    /// <summary>
    /// Initialize the hardware.
    /// </summary>
    /// <param name="device">Type of F7 board being used.</param>
    public void Initialize(F7FeatherBase device)
    {
        try
        {
            Display = new Ssd1306
            (
                i2cBus: device.CreateI2cBus(Meadow.Hardware.I2cBusSpeed.FastPlus),
                address: 60,
                displayType: Ssd1306.DisplayType.OLED128x32
            );
        }
        catch (Exception)
        {
            Display = null;
        }

        Leds = new Led[]
        {
            new Led(device.CreateDigitalOutputPort(device.Pins.D01))
            {
                IsOn = false
            },
            new Led(device.CreateDigitalOutputPort(device.Pins.D02))
            {
                IsOn = false
            },
            new Led(device.CreateDigitalOutputPort(device.Pins.D03))
            {
                IsOn = false
            },
            new Led(device.CreateDigitalOutputPort(device.Pins.D04))
            {
                IsOn = false
            },
            new Led(device.CreateDigitalOutputPort(device.Pins.D05))
            {
                IsOn = false
            },
            new Led(device.CreateDigitalOutputPort(device.Pins.D06))
            {
                IsOn = false
            },
            new Led(device.CreateDigitalOutputPort(device.Pins.D09))
            {
                IsOn = false
            },
            new Led(device.CreateDigitalOutputPort(device.Pins.D10))
            {
                IsOn = false
            }
        };
    }
}