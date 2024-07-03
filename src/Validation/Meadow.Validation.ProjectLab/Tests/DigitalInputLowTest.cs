using Meadow;
using Meadow.Devices;
using System.Threading.Tasks;

namespace Validation;

public class DigitalInputLowTest : TestDescriptor
{
    public DigitalInputLowTest(IProjectLabHardware hardware)
        : base(hardware, "PL GPIO Input to Ground", "Are all inputs working?")
    {
    }

    public override void BeginTest()
    {
        Task.Run(async () =>
        {
            using var in1 = Hardware.IOTerminal.Pins.A1.CreateDigitalInputPort(Meadow.Hardware.ResistorMode.InternalPullUp);
            using var in2 = Hardware.IOTerminal.Pins.D2.CreateDigitalInputPort(Meadow.Hardware.ResistorMode.InternalPullUp);
            using var in3 = Hardware.IOTerminal.Pins.D3.CreateDigitalInputPort(Meadow.Hardware.ResistorMode.InternalPullUp);

            while (!TestComplete)
            {
                string s = string.Empty;
                s += in1.State ? "1" : "0";
                s += in2.State ? "1" : "0";
                s += in3.State ? "1" : "0";

                RaiseUpdateInputs(s);

                await Task.Delay(1000);
            }
        });
    }
}
