using Meadow.Devices;
using Meadow.Hardware;

namespace Meadow.TestSuite
{
    internal interface ITestProvider : ITestRegistry
    {
        F7MicroBase Device { get; }

        TestInfo GetTest(string id);
    }
}