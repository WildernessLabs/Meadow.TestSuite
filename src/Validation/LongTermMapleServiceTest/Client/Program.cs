using System.Net;
using System.Net.Sockets;
using System.Text;

internal class Program
{
    private const int LOG_PORT = 5100;
    private const char LOG_DELIMITER = '\t';
    private const int HTTP_PORT = 8080;
    private const string MEADOW_ADDRESS = "192.168.100.185";

    private bool LogMemoryResults = true;
    private bool TestSimpleJson = true;

    public static async Task Main(string[] _)
    {
        Console.WriteLine("Meadow UDP Log Client");
        await new Program().Start();
    }

    private async Task Start()
    {
        new Thread(LogListener).Start();
        await RestRequester();
    }

    private async Task RestRequester()
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri($"http://{MEADOW_ADDRESS}:{HTTP_PORT}")
        };

        while (true)
        {
            try
            {
                HttpResponseMessage resp;

                if (TestSimpleJson)
                {
                    // get device-serialized json
                    resp = await client.GetAsync("/telemetry");
                }
                else
                {
                    // get random text, no serialization
                    resp = await client.GetAsync("/text?length=1024");
                }

                if (!resp.IsSuccessStatusCode)
                {
                    Console.WriteLine($"GET returned {resp.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($">> REST CALL FAILURE: {ex.Message}");
            }
            await Task.Delay(5000);
        }
    }

    private void LogListener()
    {
        UdpClient udpClient = new UdpClient();
        udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, LOG_PORT));

        var from = new IPEndPoint(0, 0);

        while (true)
        {
            var recvBuffer = udpClient.Receive(ref from);
            var payload = Encoding.UTF8.GetString(recvBuffer);
            var parts = payload.Split(new char[] { LOG_DELIMITER });
            Console.WriteLine($"{parts[0]}: {parts[1].Trim()}");

            if (LogMemoryResults)
            {
                LogToFile(payload);
            }
        }
    }

    private string _logPath = string.Empty;

    private void LogToFile(string record)
    {
        if (_logPath == string.Empty)
        {
            _logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "memlog.csv");
            using (var writer = File.CreateText(_logPath))
            {
                writer.WriteLine($"time,current,min,max,mean");
            }
        }

        if (record.Contains("MEM @"))
        {
            // extract the memory info
            var seg = record.Split(':');
            var time = TimeSpan.Parse(record.Substring(record.IndexOf("MEM @") + 5, 8));
            var current = seg[4].Split('k')[0].Trim();
            var min = seg[5].Split('k')[0].Trim();
            var max = seg[6].Split('k')[0].Trim();
            var mean = seg[7].Split('k')[0].Trim();

            using (var writer = File.AppendText(_logPath))
            {
                writer.WriteLine($"{time:hh\\:mm\\:ss},{current},{min},{max},{mean}");
            }
        }

    }
}