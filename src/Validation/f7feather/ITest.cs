using Meadow;
using System.Threading.Tasks;

namespace Validation
{
    public interface ITestFeatherF7
    {
        Task<bool> RunTest(IF7MeadowDevice device);
    }
}