using System;
using System.Collections.Generic;

namespace Meadow.TestSuite
{
    public class GetResultsCommand : TestCommand
    {
        public Guid ResultID { get; set; }
        public string TestID { get; set; }

        public GetResultsCommand()
        {
            CommandType = CommandType.GetTestResults;
        }

        public override void Execute(IWorker worker)
        {
            var results = new List<TestResult>();

            if (!ResultID.Equals(Guid.Empty))
            {
                // get result by ID
                results.Add(worker.Results.GetResult(ResultID));
            }
            else if (!string.IsNullOrEmpty(TestID))
            {
                // get results by test ID
                results.AddRange(worker.Results.GetResults(TestID));
            }
            else
            {
                // get all results
                results.AddRange(worker.Results.GetResults());
            }

            Result = results;
        }
    }
}