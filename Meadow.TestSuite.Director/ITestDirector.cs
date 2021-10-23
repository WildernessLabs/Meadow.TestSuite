using System;
using System.IO;
using System.Threading.Tasks;

namespace Meadow.TestSuite
{
    public interface ITestDirector
    {
        Task SendFile(FileInfo source, string? destinationName);
        Task<string[]> GetAssemblies();

        string DeleteAssemblies();

        string[] GetTestNames();
        TestResult[] ExecuteTests(params string[] testNames);

        TestResult[] GetTestResults();
        TestResult[] GetTestResults(string testID);
        TestResult[] GetTestResults(Guid resultID);
    }
}
