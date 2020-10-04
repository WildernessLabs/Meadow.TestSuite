using System;
using System.Collections;
using System.Collections.Generic;

namespace Meadow.TestsSuite
{
    public class TestRegistry : IEnumerable<WorkerTestDescriptor>
    {
        private Dictionary<string, WorkerTestDescriptor> _tests = new Dictionary<string, WorkerTestDescriptor>(StringComparer.InvariantCultureIgnoreCase);

        public TestRegistry()
        {

        }

        public TestDescriptor GetTest(Guid testID)
        {
            return null;
        }

        public void RegisterTest(WorkerTestDescriptor descriptor)
        {
            lock (_tests)
            {
                _tests.Add(descriptor.Name, descriptor);
            }
        }

        public void RegisterTest(string name, string area, Func<TestResult> testFunction)
        {
            lock (_tests)
            {
                var descriptor = new WorkerTestDescriptor
                {
                    Name = name,
                    TestArea = area,
                    TestFunction = testFunction
                };

                _tests.Add(descriptor.Name, descriptor);
            }
        }

        public WorkerTestDescriptor this[string name]
        {
            get
            {
                lock (_tests)
                {
                    return _tests[name];
                }
            }
        }

        public IEnumerator<WorkerTestDescriptor> GetEnumerator()
        {
            lock (_tests)
            {
                return _tests.Values.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
