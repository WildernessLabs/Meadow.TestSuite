using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Meadow.TestSuite
{
    public class RestTestDirector : ITestDirector
    {
        private HttpClient Client { get; }
        private IPEndPoint WorkerEndPoint { get; }

        public RestTestDirector(string endpoint)
        {
            if (!IPEndPoint.TryParse(endpoint, out IPEndPoint ep))
            {
                throw new ArgumentException($"Unable to parse Endpoint '{endpoint}'");
            }

            WorkerEndPoint = ep;
            Client = new HttpClient();
        }

        public RestTestDirector(IPEndPoint ep)
        {
            WorkerEndPoint = ep;
            Client = new HttpClient();
        }

        public async Task SendFile(FileInfo source, string? destinationName)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (!source.Exists)
            {
                throw new ArgumentException($"Source file '{source.FullName}' not found");
            }

            var dest = $"{WorkerEndPoint}/assemblies/{destinationName ?? source.Name}";
            var content = new StreamContent(source.OpenRead());

            await Client.PutAsync(dest, content);
        }

        public async Task<string[]> GetAssemblies()
        {
            // GET http://{{meadow-address}}:{{meadow-port}}/assemblies
            var path = $"{WorkerEndPoint}/assemblies";
            var result = await Client.GetAsync(path);
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<string[]>(json);
            }
            else
            {
                throw new Exception($"REST call returned {result.StatusCode}");
            }
            
        }

        public string DeleteAssemblies()
        {
            return "Not implemented";
        }

        public string[] GetTestNames()
        {
            throw new NotImplementedException();
        }

        public TestResult[] ExecuteTests(params string[] testNames)
        {
            throw new NotImplementedException();
        }

        public TestResult[] GetTestResults()
        {
            throw new NotImplementedException();
        }

        public TestResult[] GetTestResults(string testID)
        {
            throw new NotImplementedException();
        }

        public TestResult[] GetTestResults(Guid resultID)
        {
            throw new NotImplementedException();
        }
    }
}
