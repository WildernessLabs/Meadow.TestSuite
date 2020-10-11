using CommandLine;

namespace Meadow.TestSuite
{
    [Verb("uplink", HelpText = "Uplink a file to the Meadow worker.")]
    public class UplinkOptions : BaseOptions
    {
        [Option('s', "source", Required = true, HelpText = "Source file to uplink")]
        public string Source { get; set; }
        [Option('d', "destination", Required = false, HelpText = "Destination file name")]
        public string Destination { get; set; }
    }
}
