using Meadow.Devices;

namespace Meadow.TestSuite
{
    public interface ITestProvider : ITestRegistry
    {
        F7MicroBase Device { get; }

        TestInfo? GetTest(string id);
    }
}