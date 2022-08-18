using System;
using System.Collections.Generic;
using System.Linq;

namespace Meadow.TestSuite
{
    public class ResultsStore : IResultsStore
    {
        private Dictionary<Guid, TestResult> m_results = new Dictionary<Guid, TestResult>();

        public void Add(TestResult result)
        {
            m_results.Add(result.ResultID, result);
        }

        public TestResult GetResult(Guid resultID)
        {
            if(m_results.ContainsKey(resultID))
            {
                return m_results[resultID];
            }
            return null;
        }

        public TestResult[] GetResults(string testID)
        {
            return m_results.Values.Where(r => string.Compare(r.TestID, testID, true) == 0).ToArray();
        }

        public TestResult[] GetResults()
        {
            return m_results.Values.ToArray();
        }
    }
}