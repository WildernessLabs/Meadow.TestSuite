using System;
using System.Diagnostics;
using System.IO;

namespace Meadow.TestSuite
{
    public class TestDirector
    {
        private TestRegistry Registry { get; }
        private ITestTransport Transport { get; }

        public string MeadowTestFolder { get; } = "/meadow0/test";

        public TestDirector(ITestTransport transport)
        {
            Transport = transport;
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

            Transport.DeliverCommand(cmd);
        }

        public void DiscoverTests()
        {

        }
    }
}
