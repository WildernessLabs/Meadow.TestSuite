
using Meadow;
using Meadow.Hardware;

namespace Meadow.Validation
{
    public class UniDirectionAB : IPairTest
    {
        public bool RunTest(PinPair pair)
        {
            using(var output = pair.Device.CreateDigitalOutputPort(pair.A, false))
            using(var input = pair.Device.CreateDigitalInputPort(pair.B, InterruptMode.None, ResistorMode.InternalPullDown))
            {
                var success = true;

                // should be low
                success &= input.State == false;
                Resolver.Log.ErrorIf(!success, $"PIN {input.Pin.Name} was expected to be false but was {input.State}");

                output.State = true;

                // should be high
                success &= input.State == true;
                Resolver.Log.ErrorIf(!success, $"PIN {input.Pin.Name} was expected to be true but was {input.State}");

                output.State = false;

                // should be low
                success &= input.State == false;
                Resolver.Log.ErrorIf(!success, $"PIN {input.Pin.Name} was expected to be false but was {input.State}");

                return success;
            }
        }
    }
}