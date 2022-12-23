using System.Threading.Tasks;

namespace Meadow.Validation
{
    public interface IDeviceUnderTest<T> where T : IMeadowDevice
    {
        public T Device { get; }
    }

    public class F7TestDevice : IDeviceUnderTest<IMeadowDevice>
    {
        public IMeadowDevice Device { get; }

        public F7TestDevice(IMeadowDevice device)
        {
            Device = device;
        }
    }

    public interface ITest<T> where T : IDeviceUnderTest<IMeadowDevice>
    {
        Task<bool> RunTest(T device);
    }
}