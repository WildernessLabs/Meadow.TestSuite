using Meadow;
using Meadow.Devices;
using System.Threading.Tasks;

namespace Validation;

public class DigitalOutputTest : TestDescriptor
{
    public DigitalOutputTest(IProjectLabHardware hardware)
        : base(hardware, "PL GPIO Outputs", "Are all 3 outputs toggling?")
    {
    }

    public override void BeginTest()
    {
        Task.Run(async () =>
        {
            using var out1 = Hardware.IOTerminal.Pins.A1.CreateDigitalOutputPort(true);
            using var out2 = Hardware.IOTerminal.Pins.D2.CreateDigitalOutputPort(false);
            using var out3 = Hardware.IOTerminal.Pins.D3.CreateDigitalOutputPort(true);

            while (!TestComplete)
            {

                out1.State = !out1.State;
                out2.State = !out2.State;
                out3.State = !out3.State;

                await Task.Delay(1000);
            }
        });
    }
}
