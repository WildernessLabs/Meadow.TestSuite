using Meadow.Devices;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;

namespace FeatherF7Test.Hardware;
internal interface IOLEDBoardHardware
{
    public IGraphicsDisplay Display { get; }

    public Led Led1 { get; }

    public Led Led2 { get; }

    public Led Led3 { get; }

    public Led Led4 { get; }

    public void Initialize(F7FeatherBase device);
}