using Meadow.Foundation.Sensors.Buttons;
using Meadow.Hardware;

namespace Validation
{
    public class InspectablePushButton : PushButton
    {
        public InspectablePushButton(IDigitalInputPort input)
            : base(input)
        {
        }

        public string PinName => DigitalIn.Pin.Name;
    }
}