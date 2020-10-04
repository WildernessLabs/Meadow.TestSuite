using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Meadow.TestsSuite
{
    public class TestRunner
    {
        public TestResult Result { get; private set; }
        public WorkerTestDescriptor Descriptor { get; }

        public TestRunner(WorkerTestDescriptor test)
        {
            if (test == null)
            {
                throw new ArgumentNullException();
            }

            Descriptor = test;
        }

        public async Task<TestResult> Execute()
        {
            await Task.Run(() =>
            {
                var sw = new Stopwatch();
                sw.Start();
                Result = Descriptor.TestFunction();
                sw.Stop();

                Result.ResultID = Guid.NewGuid();
                Result.TestID = Descriptor.TestID;
                Result.CompletionDate = DateTime.Now;
                Result.RunTimeSeconds = sw.ElapsedMilliseconds / 1000d;
            });

            return Result;
        }
    }
}
