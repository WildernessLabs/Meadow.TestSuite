using Meadow.Devices;
using System.Threading.Tasks;

namespace Validation;

public class AccelerometerTest : TestDescriptor
{
    public AccelerometerTest(IProjectLabHardware hardware)
        : base(hardware, "PL Accelerometer", "Is the accelerometer working?")
    {
    }

    public override void BeginTest()
    {
        Task.Run(async () =>
        {
            while (!TestComplete)
            {
                var read = await Hardware.Accelerometer.Read();

                RaiseUpdateInputs($"X:{read.X:N2} Y:{read.Y:N2} Z:{read.Z:N2}");

                await Task.Delay(1000);
            }
        });
    }
}
