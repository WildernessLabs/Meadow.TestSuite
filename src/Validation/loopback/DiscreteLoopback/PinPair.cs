
using Meadow;
using Meadow.Hardware;

namespace Validation
{
    public class PinPair
    {
        public IMeadowDevice Device { get; }
        public IPin A { get; }
        public IPin B { get; }

        public PinPair(IMeadowDevice device, IPin a, IPin b)
        {
            Device = device;
            A = a;
            B = b;
        }
    }
}