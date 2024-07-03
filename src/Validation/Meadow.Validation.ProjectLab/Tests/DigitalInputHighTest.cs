using Meadow;
using Meadow.Devices;
using System.Threading.Tasks;

namespace Validation;

public class DigitalInputHighTest : TestDescriptor
{
    public DigitalInputHighTest(IProjectLabHardware hardware)
        : base(hardware, "PL GPIO Input to 3.3V", "Are all inputs working?")
    {
    }

    public override void BeginTest()
    {
        Task.Run(async () =>
        {
            using var in1 = Hardware.IOTerminal.Pins.A1.CreateDigitalInputPort(Meadow.Hardware.ResistorMode.InternalPullDown);
            using var in2 = Hardware.IOTerminal.Pins.D2.CreateDigitalInputPort(Meadow.Hardware.ResistorMode.InternalPullDown);
            using var in3 = Hardware.IOTerminal.Pins.D3.CreateDigitalInputPort(Meadow.Hardware.ResistorMode.InternalPullDown);

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
