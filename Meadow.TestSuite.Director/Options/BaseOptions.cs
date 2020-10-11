using CommandLine;

namespace Meadow.TestSuite
{
    public class BaseOptions : IOptions
    {
        [Option('p', "port", Required = true, HelpText = "Serial Port (e.g. COM12)")]
        public string Port { get; set; }
        [Option('b', "baud-rate", Required = false, HelpText = "Baud rate.  Must match the Worker.")]
        public int BaudRate { get; set; } = 9600;
    }
}
