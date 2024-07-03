using Meadow.Devices;

namespace Validation;

public class SpiWriteTest : TestDescriptor
{
    public SpiWriteTest(IProjectLabHardware hardware)
        : base(hardware, "SPI Write", "Is this text visible?")
    {
    }

    public override void BeginTest()
    {
        // NOP
    }
}
