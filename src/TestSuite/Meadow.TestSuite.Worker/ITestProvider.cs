using Meadow.Devices;
using Meadow.Hardware;

namespace Meadow.TestSuite
{
    public interface ITestProvider : ITestRegistry
    {
        F7MicroBase Device { get; }

        TestInfo GetTest(string id);
    }
}