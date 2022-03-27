using Meadow.Devices;
using Meadow.Logging;

namespace Munit
{
    public class MeadowTestBase
    {
        public IMeadowDevice Device { get; set; }
        public Logger Logger { get; set; }
    }
}
