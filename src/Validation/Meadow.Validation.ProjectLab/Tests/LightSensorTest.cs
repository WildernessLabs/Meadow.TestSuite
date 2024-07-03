using Meadow.Devices;
using System.Threading.Tasks;

namespace Validation;

public class LightSensorTest : TestDescriptor
{
    public LightSensorTest(IProjectLabHardware hardware)
        : base(hardware, "PL Light Sensor", "Is the light sensor working?")
    {
    }

    public override void BeginTest()
    {
        Task.Run(async () =>
        {
            while (!TestComplete)
            {
                var read = await Hardware.LightSensor.Read();

                RaiseUpdateInputs($"{read.Lux:N0} lux");

                await Task.Delay(1000);
            }
        });
    }
}
