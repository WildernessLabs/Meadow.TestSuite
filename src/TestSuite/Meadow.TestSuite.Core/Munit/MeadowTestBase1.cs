using Meadow;

namespace Munit
{
    public class MeadowTestBase<T> : MeadowTestBase
        where T : IMeadowDevice
    {
        public new T Device { get; set; }
    }
}
