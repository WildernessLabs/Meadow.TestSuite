using System;
using System.Net;

namespace Meadow.TestSuite.Cli
{
    public class DirectorProvider
    {
        public DirectorProvider()
        {
        }

        public ITestDirector GetDirector(string declaration)
        {
            // try to parse to an endpoint
            if (IPEndPoint.TryParse(declaration, out IPEndPoint ep))
            {
                Console.WriteLine($"Connecting to DUT using ethernet '{ep.Address}'");

                return new RestTestDirector(ep);
            }
            else
            {
                // else try serial
                var serializer = new CommandJsonSerializer();
                serializer.UseLibrary = JsonLibrary.SystemTextJson;

                // TODO: figure a better way for this formatting
                // for now we use [port]:[rate]
                var segments = declaration.Split(":");
                var port = segments[0];
                int baudRate;
                if (segments.Length > 1)
                {
                    baudRate = int.Parse(segments[1]);
                }
                else
                {
                    baudRate = 115200;
                }

                // TODO: use a log provider
                Console.WriteLine($"Connecting to DUT using serial port '{port}'");

                var transport = new WorkerSerialTransport(serializer, port, baudRate);
                return new SerialTestDirector(serializer, transport);
            }
        }
    }
}