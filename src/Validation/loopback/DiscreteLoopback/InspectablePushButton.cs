using Meadow.Foundation.Sensors.Buttons;
using Meadow.Hardware;

namespace Meadow.Validation
{
    public class InspectablePushButton : PushButton
    {
        public InspectablePushButton(IDigitalInterruptPort input)
            : base(input)
        {
        }

        public string PinName => DigitalIn.Pin.Name;
    }
}