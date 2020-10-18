using System;

namespace Meadow.TestSuite
{
    public interface IResultsStore
    {
        TestResult GetResult(Guid resultID);
        TestResult[] GetResults(string testID);
        TestResult[] GetResults();
    }
}