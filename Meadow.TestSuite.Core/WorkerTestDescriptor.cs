using System;

namespace Meadow.TestSuite
{
    public interface IExecutableTest
    {
        Func<TestResult> TestFunction { get; }
        string Name { get; }
    }

    public class WorkerTestDescriptor : TestDescriptor
    {
        public Func<TestResult> TestFunction { get; set; }

        public TestRunner GetRunner()
        {
            return new TestRunner(this);
        }
    }
}
