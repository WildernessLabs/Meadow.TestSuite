
using Meadow;
using Meadow.Hardware;
using System;
using System.Threading;

namespace Meadow.Validation
{
    public abstract class InterruptBase : IPairTest
    {
        public abstract bool RunTest(PinPair pair);

        protected bool RunTest(PinPair pair, InterruptMode mode, bool aIsOutput)
        {
            if(aIsOutput)
            {
                if(!pair.B.Supports<IDigitalChannelInfo>(c => c.InterruptCapable))
                {
                    Resolver.Log.Warn($"PIN {pair.B.Name} does not support interrupts. Skipping.");
                    return true;
                }
            }
            else
            {
                if(!pair.A.Supports<IDigitalChannelInfo>(c => c.InterruptCapable))
                {
                    Resolver.Log.Warn($"PIN {pair.A.Name} does not support interrupts. Skipping.");
                    return true;
                }
            }

            using(var output = pair.Device.CreateDigitalOutputPort(aIsOutput ? pair.A : pair.B, false))
            using(var input = pair.Device.CreateDigitalInputPort(aIsOutput ? pair.B : pair.A, mode, ResistorMode.InternalPullDown))
            {
                var success = true;
                var interruptCount = 0;

                input.Changed += (s, e) => ++interruptCount;

                output.State = true;
                output.State = false;

                var delay = 0;

                while(interruptCount == 0)
                {
                    Thread.Sleep(10);
                    delay += 10;
                    if(delay > 50) break;
                }

                if(delay > 0)
                {
                    Resolver.Log.Warn($"Slow interrupt on {pair.B.Name} (took {delay}ms)");
                }

                string direction;

                switch(mode)
                {
                    case InterruptMode.EdgeRising:
                        success = interruptCount == 1;
                        direction = "a single rising interrupt";
                        break;
                    case InterruptMode.EdgeFalling:
                        success = interruptCount == 1;
                        direction = "a single falling interrupt";
                        break;
                    case InterruptMode.EdgeBoth:
                        success = interruptCount == 2;
                        direction = "two interrupts";
                        break;
                    default:
                        throw new NotSupportedException();
                }

                Resolver.Log.ErrorIf(!success, $"PIN {input.Pin.Name} expected {direction} but received {interruptCount}");

                return success;
            }
        }
    }
}