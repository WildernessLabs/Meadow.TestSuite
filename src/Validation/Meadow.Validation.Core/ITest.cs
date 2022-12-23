using System.Threading.Tasks;

namespace Meadow.Validation
{
    public interface IDeviceUnderTest
    {
        public IMeadowDevice Device { get; }
    }

    public class F7TestDevice : IDeviceUnderTest
    {
        public IMeadowDevice Device { get; }

        public F7TestDevice(IMeadowDevice device)
        {
            Device = device;
        }
    }

    public interface ITest<T> where T : IDeviceUnderTest
    {
        Task<bool> RunTest(T device);
    }
}