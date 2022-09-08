using Meadow;
using Meadow.Devices;
using System;
using System.Threading.Tasks;

namespace Validation
{
    public class I2CBusTest : ITest
    {
        public async Task<bool> RunTest(IMeadowDevice device, ProjectLab projectLab)
        {
            // connect to the light sensor on the bus to verify it's working

            try
            {
                projectLab.LightSensor.StartUpdating();
                var lux = await projectLab.LightSensor.Read();
                Resolver.Log.Debug($"Lux: {lux}");
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"{this.GetType().Name} failed: {ex.Message}");
                return false;
            }

            return true;
        }
    }
}