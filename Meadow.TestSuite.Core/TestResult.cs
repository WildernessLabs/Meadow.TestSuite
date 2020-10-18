using System;
using System.Collections.Generic;

namespace Meadow.TestSuite
{
    public class TestResult
    {
        public TestResult()
        {            
        }

        public TestResult(string testID)
        {
            TestID = testID;
            State = TestState.NotRun;
        }

        public Guid ResultID { get; set; }
        public string TestID { get; set; }
        public double? RunTimeSeconds { get; set; }
        public DateTime? CompletionDate { get; set; }
        public TestState State { get; set; }
        public List<string> Output { get; set; } = new List<string>();

        public override string ToString()
        {
            return $"{State}:\t{TestID}";
        }
    }
}
