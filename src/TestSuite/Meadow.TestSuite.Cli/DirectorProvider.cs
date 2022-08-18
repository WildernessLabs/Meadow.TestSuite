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
                var transport = new WorkerSerialTransport(serializer, segments[0], int.Parse(segments[1]));
                return new SerialTestDirector(serializer, transport);
            }            
        }
    }
}