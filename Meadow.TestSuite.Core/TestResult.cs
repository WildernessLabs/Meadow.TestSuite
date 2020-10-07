using System;
using System.Collections.Generic;

namespace Meadow.TestSuite
{
    public class TestResult
    {
        public Guid ResultID { get; set; }
        public Guid TestID { get; set; }
        public double? RunTimeSeconds { get; set; }
        public DateTime? CompletionDate { get; set; }
        public TestState State { get; set; }
        public List<string> Output { get; set; } = new List<string>();
    }
}
