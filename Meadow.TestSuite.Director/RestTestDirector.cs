using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Meadow.TestSuite
{
    public class RestTestDirector : ITestDirector
    {
        private HttpClient Client { get; set; }
        private IPEndPoint WorkerEndPoint { get; set; }
        private JsonSerializerOptions m_options;

        public RestTestDirector(string endpoint)
        {
            if (!IPEndPoint.TryParse(endpoint, out IPEndPoint ep))
            {
                throw new ArgumentException($"Unable to parse Endpoint '{endpoint}'");
            }

            Init(ep);
        }

        public RestTestDirector(IPEndPoint ep)
        {
            Init(ep);
        }

        private void Init(IPEndPoint ep)
        {
            WorkerEndPoint = ep;
            Client = new HttpClient();
            Client.BaseAddress = new Uri($"http://{ep}");
            m_options = new JsonSerializerOptions
            {
                 PropertyNameCaseInsensitive = true
            };
            m_options.Converters.Add(new JsonStringEnumConverter());
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

            var dest = $"/assemblies/{destinationName ?? source.Name}";
            var content = new StreamContent(source.OpenRead());

            await Client.PutAsync(dest, content);
        }

        public async Task<string[]> GetAssemblies()
        {
            // GET http://{{meadow-address}}:{{meadow-port}}/assemblies
            var path = "/assemblies";
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

        public async Task<string[]> GetTestNames()
        {
            // GET http://{{meadow-address}}:{{meadow-port}}/tests
            var path = "/tests";
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

        public async Task<TestResult> ExecuteTest(string testName)
        {
            // GET http://{{meadow-address}}:{{meadow-port}}/tests
            var path = $"/tests/{testName}";
            var result = await Client.PostAsync(path, null);
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TestResult>(json, m_options);
            }
            else
            {
                throw new Exception($"REST call returned {result.StatusCode}");
            }
        }

        public async Task<TestResult[]> GetTestResults()
        {
            var path = "/results";
            var result = await Client.GetAsync(path);
            if (result.IsSuccessStatusCode)
            {
                var json = await result.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TestResult[]>(json, m_options);
            }
            else
            {
                throw new Exception($"REST call returned {result.StatusCode}");
            }
        }

        public Task<TestResult[]> GetTestResults(string testID)
        {
            throw new NotImplementedException();
        }

        public Task<TestResult[]> GetTestResults(Guid resultID)
        {
            throw new NotImplementedException();
        }
    }
}
