using Meadow.Devices;
using System.Threading.Tasks;

namespace Validation;

public class TempSensorTest : TestDescriptor
{
    public TempSensorTest(IProjectLabHardware hardware)
        : base(hardware, "PL Temp Sensor", "Is the temp sensor working?")
    {
    }

    public override void BeginTest()
    {
        Task.Run(async () =>
        {
            while (!TestComplete)
            {
                var read = await Hardware.TemperatureSensor.Read();

                RaiseUpdateInputs($"{read.Celsius:N1} C / {read.Fahrenheit:N1} F");

                await Task.Delay(1000);
            }
        });
    }
}
