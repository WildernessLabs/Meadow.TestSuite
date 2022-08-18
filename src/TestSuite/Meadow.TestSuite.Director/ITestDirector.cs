using System;
using System.IO;
using System.Threading.Tasks;

namespace Meadow.TestSuite
{
    public interface ITestDirector
    {
        Task<WorkerInfo> GetInfo();
        Task<DateTime> GetTime();
        Task SetTime(DateTime time);

        Task SendFile(FileInfo source, string? destinationName);
        Task<string[]> GetAssemblies();

        Task<string[]> GetTestNames();        
        Task<TestResult> ExecuteTest(string testName);

        Task<TestResult[]> GetTestResults();
        Task<TestResult[]> GetTestResults(string testID);
        Task<TestResult> GetTestResults(Guid resultID);
    }
}
