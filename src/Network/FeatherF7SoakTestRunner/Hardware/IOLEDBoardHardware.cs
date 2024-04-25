using Meadow.Devices;
using Meadow.Foundation.Leds;
using Meadow.Peripherals.Displays;

namespace FeatherF7Test.Hardware;
internal interface IOLEDBoardHardware
{
    /// <summary>
    /// Display used to show progress.
    /// </summary>
    public IPixelDisplay Display { get; }

    /// <summary>
    /// LEDs that can be used to indicate test progress / status.
    /// </summary>
    public Led[] Leds { get; }

    /// <summary>
    /// Initialize the hardware.
    /// </summary>
    /// <param name="device">Type of device the test is running on.</param>
    public void Initialize(F7FeatherBase device);
}