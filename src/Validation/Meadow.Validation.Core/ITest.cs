using System.Threading.Tasks;

namespace Meadow.Validation
{
    public interface IDeviceUnderTest<T> where T : class, IMeadowDevice
    {
        public T Device { get; }
    }

    public class MeadowTestDevice : IDeviceUnderTest<IMeadowDevice>
    {
        public IMeadowDevice Device { get; }

        public MeadowTestDevice(IMeadowDevice device)
        {
            Device = device;
        }
    }

    public interface ITest<T> where T : IDeviceUnderTest<IMeadowDevice>
    {
        Task<bool> RunTest(T device);
    }
}