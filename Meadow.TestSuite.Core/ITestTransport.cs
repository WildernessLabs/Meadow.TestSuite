using System;
using System.Collections.Generic;

namespace Meadow.TestsSuite
{
    public interface ITestTransport
    {
        void DeliverCommand(TestCommand command);

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

    public class WorkerLocalTransport : ITestTransport
    {
        private TestRegistry Registry { get; }

        public WorkerLocalTransport(TestRegistry registry)
        {
            Registry = registry;
        }

        public void EnqueueTest(Guid testID)
        {
            var test = Registry.GetTest(testID);
        }

        public IEnumerable<TestResult> GetResults()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TestResult> GetResults(string area)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TestResult> GetResults(Guid testID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetTestAreas()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TestDescriptor> GetTestList()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TestDescriptor> GetTestList(string area)
        {
            throw new NotImplementedException();
        }

        public void DeliverCommand(TestCommand command)
        {
            throw new NotImplementedException();
        }
    }

    public interface ITestReporter
    {

    }
}
