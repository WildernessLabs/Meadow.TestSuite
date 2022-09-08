
using Meadow;
using Meadow.Devices;
using System.Threading.Tasks;

namespace Validation
{
    public interface ITest
    {
        Task<bool> RunTest(IMeadowDevice device, ProjectLab projectLab);
    }
}