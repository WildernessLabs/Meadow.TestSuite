using System;
using System.Collections.Generic;

namespace Meadow.TestSuite
{
    public interface ITestTransport
    {
        byte[] DeliverCommand(TestCommand command);

        /*
        IEnumerable<string> GetTestAreas();
        IEnumerable<TestDescriptor> GetTestList();
        IEnumerable<TestDescriptor> GetTestList(string area);
        IEnumerable<TestResult> GetResults();
        IEnumerable<TestResult> GetResults(string area);
        IEnumerable<TestResult> GetResults(Guid testID);

        void EnqueueTest(Guid testID);
        */
    }
}
