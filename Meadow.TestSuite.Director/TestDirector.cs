using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Meadow.TestSuite
{
    public class TestDirector
    {
        private TestRegistry Registry { get; }
        private ITestTransport Transport { get; }
        private ICommandSerializer Serializer { get; }

        public string MeadowTestFolder { get; } = "/meadow0/test";

        public TestDirector(ICommandSerializer serializer, ITestTransport transport)
        {
            Transport = transport;
            Serializer = serializer;

            Registry = new TestRegistry();
        }

        public void UplinkTestAssembly(string assemblyPath)
        {
            var fi = new FileInfo(assemblyPath);
            if(!fi.Exists)
            {
                throw new FileNotFoundException("Source file not found");
            }

            var destpath = Path.Combine(MeadowTestFolder, fi.Name)
                .Replace('\\', '/');

            var payload = Convert.ToBase64String(File.ReadAllBytes(fi.FullName));
            Debug.WriteLine($"Payload is {payload.Length} bytes");
            var cmd = new UplinkFileCommand
            {
                Destination = destpath,
                FileData = payload
            };

            var result = Transport.DeliverCommand(cmd);
            ProcessResults(result);
        }

        public void GetAssemblies()
        {
            var cmd = new GetAssembliesCommand();
            var result = Transport.DeliverCommand(cmd);
            ProcessResults(result);
        }

        public void GetTestNames()
        {
            var cmd = new GetTestNamesCommand();
            var result = Transport.DeliverCommand(cmd);
            ProcessResults(result);
        }

        public void ExecuteTests(params string[] testNames)
        {
            var cmd = new ExecuteTestsCommand(testNames);
            var result = Transport.DeliverCommand(cmd);
            ProcessResults(result);
        }

        public void ProcessResults(byte[] result)
        {
            // TODO: run through the serializer to get the result
            if (result == null)
            {
                Console.WriteLine("Null result");
            }
            else
            {
                Console.WriteLine(Encoding.UTF8.GetString(result));
            }
        }

        public void DiscoverTests()
        {

        }
    }
}
