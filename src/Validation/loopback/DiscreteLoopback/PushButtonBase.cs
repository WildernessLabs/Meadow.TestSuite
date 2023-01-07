
using Meadow;
using Meadow.Hardware;
using System.Threading;

namespace Meadow.Validation
{
    public abstract class PushButtonBase : IPairTest
    {
        public abstract bool RunTest(PinPair pair);

        public bool RunTest(PinPair pair, bool aIsOutput)
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
            using(var input = pair.Device.CreateDigitalInputPort(aIsOutput ? pair.B : pair.A, InterruptMode.EdgeBoth, ResistorMode.InternalPullDown))
            using(var button = new InspectablePushButton(input))
            {
                return TestButton(output, button);
            }
        }

        protected bool TestButton(IDigitalOutputPort output, InspectablePushButton button)
        {
            var startReceived = false;
            var stopReceived = false;
            var clickReceived = false;
            var longClickReceived = false;

            var success = true;

            button.PressStarted += (s, e) => startReceived = true;
            button.PressEnded += (s, e) => stopReceived = true;
            button.Clicked += (s, e) => clickReceived = true;
            button.LongClicked += (s, e) => longClickReceived = true;

            // test short click
            output.State = true;
            Thread.Sleep(5);
            output.State = false;

            // sleep longer than "long"
            Thread.Sleep((int)button.LongClickedThreshold.TotalMilliseconds + 50);

            success &= startReceived;
            Resolver.Log.ErrorIf(!startReceived, $"PushButton on Pin {button.PinName} did not get a PressStarted for short press");
            success &= stopReceived;
            Resolver.Log.ErrorIf(!stopReceived, $"PushButton on Pin {button.PinName} did not get a PressEnded for short press");
            success &= clickReceived;
            Resolver.Log.ErrorIf(!clickReceived, $"PushButton on Pin {button.PinName} did not get a Clicked");
            success &= !longClickReceived;
            Resolver.Log.ErrorIf(longClickReceived, $"PushButton on Pin {button.PinName} got a LongClicked and shouldn't have");

            // reset the flags
            startReceived = false;
            stopReceived = false;
            clickReceived = false;
            longClickReceived = false;

            // test long click
            output.State = true;
            Thread.Sleep((int)button.LongClickedThreshold.TotalMilliseconds + 10);
            output.State = false;

            // sleep longer than "long"
            Thread.Sleep((int)button.LongClickedThreshold.TotalMilliseconds + 50);

            success &= startReceived;
            Resolver.Log.ErrorIf(!startReceived, $"PushButton on Pin {button.PinName} did not get a PressStarted for long press");
            success &= stopReceived;
            Resolver.Log.ErrorIf(!stopReceived, $"PushButton on Pin {button.PinName} did not get a PressEnded for long press");
            success &= !clickReceived;
            Resolver.Log.ErrorIf(clickReceived, $"PushButton on Pin {button.PinName} got a Clicked and shouldn't have");
            success &= longClickReceived;
            Resolver.Log.ErrorIf(!longClickReceived, $"PushButton on Pin {button.PinName} did not get a LongClicked");

            return success;
        }
    }
}