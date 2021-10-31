using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Meadow.TestSuite
{
    public class SerialTestDirector : ITestDirector
    {
        private WorkerSerialTransport Transport { get; }
        private ICommandSerializer Serializer { get; }

        public string MeadowTestFolder { get; } = "/meadow0/test";

        public SerialTestDirector(ICommandSerializer serializer, WorkerSerialTransport transport)
        {
            Transport = transport;
            Serializer = serializer;
        }

        public async Task SendFile(FileInfo source, string? destinationName)
        {
            if(!source.Exists)
            {
                throw new FileNotFoundException("Source file not found");
            }

            if (destinationName == string.Empty) destinationName = null;

            var destpath = Path.Combine(MeadowTestFolder, destinationName ?? source.Name)
                .Replace('\\', '/');

            var payload = Convert.ToBase64String(File.ReadAllBytes(source.FullName));
            Debug.WriteLine($"Payload is {payload.Length} bytes");
            var cmd = new UplinkFileCommand
            {
                Destination = destpath,
                FileData = payload
            };

            await Transport.DeliverCommandAsync(cmd);
        }

        public async Task<string[]> GetAssemblies()
        {
            var cmd = new GetAssembliesCommand();
            var result = await Transport.DeliverCommandAsync(cmd);
            var assemblies = ProcessResults<string[]>(result);
            return assemblies;
        }

        public async Task<TestResult[]> GetTestResults()
        {
            var cmd = new GetResultsCommand();
            var result = await Transport.DeliverCommandAsync(cmd);
            return ProcessResults<TestResult[]>(result);
        }

        public async Task<TestResult[]> GetTestResults(string testID)
        {
            var cmd = new GetResultsCommand()
            {
                TestID = testID
            };
            var result = await Transport.DeliverCommandAsync(cmd);
            return ProcessResults<TestResult[]>(result);
        }

        public async Task<TestResult[]> GetTestResults(Guid resultID)
        {
            var cmd = new GetResultsCommand()
            {
                ResultID = resultID
            };
            var result = await Transport.DeliverCommandAsync(cmd);
            return ProcessResults<TestResult[]>(result);
        }

        public string DeleteAssemblies()
        {
            var cmd = new DeleteFileCommand()
            {
                Path = MeadowTestFolder,
                Pattern = "*"
            };

            var result = Transport.DeliverCommand(cmd);
            return ProcessResults<string>(result);
        }

        public async Task<string[]> GetTestNames()
        {
            var cmd = new GetTestNamesCommand();
            var result = await Transport.DeliverCommandAsync(cmd);
            return ProcessResults<string[]>(result);
        }

        public async Task<TestResult> ExecuteTest(string testName)
        {
            return (await ExecuteTests(testName))[0];
        }

        public async Task<TestResult[]> ExecuteTests(params string[] testNames)
        {
            var cmd = new ExecuteTestsCommand(testNames);
            var result = await Transport.DeliverCommandAsync(cmd);
            return ProcessResults<TestResult[]>(result);
        }

        public TResult ProcessResults<TResult>(byte[] result)
        {
            // TODO: run through the serializer to get the result
            if (result == null)
            {
                Console.WriteLine("Null result");
                return default(TResult);
            }
            else
            {
                return Serializer.Deserialize<TResult>(result);
            }
        }

        public void DiscoverTests()
        {

        }
    }
}
