
using Meadow;
using Meadow.Devices;
using System.Threading.Tasks;

namespace Validation
{
    public class SpiBusTest : ITest
    {
        public Task<bool> RunTest(IMeadowDevice device, ProjectLab projectLab)
        {

            // TODO: connect to something on the bus to verify it's working

            return Task.FromResult(false);
        }
    }
}