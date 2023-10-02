using Meadow.Hardware;
using System.Threading;

namespace Meadow.Validation
{
    public abstract class CounterBase : IPairTest
    {
        public abstract bool RunTest(PinPair pair);

        public const int CountsToTest = 50;

        protected bool RunTest(PinPair pair, InterruptMode mode, bool aIsOutput)
        {
            if (aIsOutput)
            {
                if (!pair.B.Supports<IDigitalChannelInfo>(c => c.InterruptCapable))
                {
                    Resolver.Log.Warn($"PIN {pair.B.Name} does not support interrupts. Skipping.");
                    return true;
                }
            }
            else
            {
                if (!pair.A.Supports<IDigitalChannelInfo>(c => c.InterruptCapable))
                {
                    Resolver.Log.Warn($"PIN {pair.A.Name} does not support interrupts. Skipping.");
                    return true;
                }
            }

            using (var output = pair.Device.CreateDigitalOutputPort(aIsOutput ? pair.A : pair.B, false))
            using (var input = pair.Device.CreateDigitalInterruptPort(aIsOutput ? pair.B : pair.A, mode, ResistorMode.InternalPullDown))
            using (var counter = new Counter(input))
            {
                var success = true;

                counter.Enabled = false;

                // make sure it doesn't count while disabled
                for (int i = 0; i < CountsToTest; i++)
                {
                    switch (mode)
                    {
                        case InterruptMode.EdgeRising:
                            output.State = false;
                            output.State = true;
                            break;
                        case InterruptMode.EdgeFalling:
                            output.State = true;
                            output.State = false;
                            break;
                        case InterruptMode.EdgeBoth:
                            output.State = !output.State;
                            break;
                    }

                    // not sure when the behavior changed, but as of v 1.0.0, this is required or the interrupts just never get serviced
                    Thread.Sleep(1);
                }

                success &= counter.Count == 0;

                Resolver.Log.ErrorIf(!success, $"PIN {output.Pin.Name} expected 0 events but received {counter.Count}");

                // not sure when the behavior changed, but as of v 1.0.0, this is required to prevent previous interupts from "bleeding" into the next set
                Thread.Sleep(100);

                counter.Enabled = true;

                // should count while enabled
                for (int i = 0; i < CountsToTest; i++)
                {
                    switch (mode)
                    {
                        case InterruptMode.EdgeRising:
                            output.State = false;
                            output.State = true;
                            break;
                        case InterruptMode.EdgeFalling:
                            output.State = true;
                            output.State = false;
                            break;
                        case InterruptMode.EdgeBoth:
                            output.State = !output.State;
                            break;
                    }

                    // not sure when the behavior changed, but as of v 1.0.0, this is required or the interrupts just never get serviced
                    Thread.Sleep(1);
                }

                success &= counter.Count == CountsToTest;

                Resolver.Log.ErrorIf(counter.Count != CountsToTest, $"PIN {output.Pin.Name} expected {CountsToTest} events but received {counter.Count}");

                // test reset
                counter.Reset();
                success &= counter.Count == 0;

                Resolver.Log.ErrorIf(counter.Count != 0, $"Counter expected 0 events but received {counter.Count}");

                return success;
            }
        }
    }
}