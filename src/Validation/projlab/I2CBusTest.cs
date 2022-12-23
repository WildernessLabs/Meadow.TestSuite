using System;
using System.Threading.Tasks;

namespace Meadow.Validation
{
    public class I2CBusTest<T> : ITest<T>
        where T : ProjectLabTestDevice
    {
        public async Task<bool> RunTest(T device)
        {
            // connect to the light sensor on the bus to verify it's working

            try
            {
                device.ProjectLab.LightSensor.StartUpdating();
                var lux = await device.ProjectLab.LightSensor.Read();
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