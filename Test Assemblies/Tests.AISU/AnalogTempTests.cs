using System.Threading;
using Meadow.Devices;
using Meadow.Foundation.Sensors.Temperature;
using Meadow.Hardware;
using Munit;

namespace MeadowLibary
{
    public class AnalogTempTests
    {
        public F7MicroBase Device { get; set; }

        [Fact]
        public async void TestAnalogTemps()
        {
            var sensor = new AnalogTemperature(
                device: Device,
                analogPin: Device.Pins.A00,
                sensorType: AnalogTemperature.KnownSensorType.LM35
            );

            Assert.NotNull(sensor);
            await sensor.Read();
            Assert.True(sensor.Temperature.HasValue);

            // this assumes our physical test environment is somewhat sane
            Assert.True(sensor.Temperature.Value.Fahrenheit > 60);
            Assert.True(sensor.Temperature.Value.Fahrenheit < 90);
        }
    }
}