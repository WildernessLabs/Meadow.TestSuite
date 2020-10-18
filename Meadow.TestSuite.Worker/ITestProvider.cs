using Meadow.Hardware;

namespace Meadow.TestSuite
{
    internal interface ITestProvider : ITestRegistry
    {
        IIODevice Device { get; }

        TestInfo GetTest(string id);
    }
}