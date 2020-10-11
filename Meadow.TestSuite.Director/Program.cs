using CommandLine;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TestSuite.Unit.Tests")]

namespace Meadow.TestSuite
{
    public interface IOptions
    {
        string Port { get; }
        int BaudRate { get; }
    }

    public class BaseOptions : IOptions
    {
        [Option('p', "port", Required = true, HelpText = "Serial Port (e.g. COM12)")]
        public string Port { get; set; }
        [Option('b', "baud-rate", Required = false, HelpText = "Baud rate.  Must match the Worker.")]
        public int BaudRate { get; set; } = 9600;
    }

    [Verb("uplink", HelpText = "Uplink a file to the Meadow worker.")]
    public class UplinkOptions : BaseOptions
    {
        [Option('s', "source", Required = true, HelpText = "Source file to uplink")]
        public string Source { get; set; }
        [Option('d', "destination", Required = false, HelpText = "Destination file name")]
        public string Destination { get; set; }
    }

    [Verb("assembly", HelpText = "Assembly commands")]
    public class AssemblyOptions : BaseOptions
    {
        [Option('l', "list", Required = false, HelpText = "List All Known Assemlies")]
        public bool List { get; set; }
    }

    [Verb("test", HelpText = "Test Commands")]
    public class TestOptions : BaseOptions
    {
        [Option('l', "list", Required = false, HelpText = "List All Known Tests")]
        public bool List { get; set; }
        [Option('e', "execute", Required = false, HelpText = "Execute the specified test")]
        public string Execute { get; set; }
    }

    [Verb("result", HelpText = "Result Commands")]
    public class ResultOptions : BaseOptions
    {
        [Option('l', "list", Required = false, HelpText = "List All Known Test Results")]
        public bool List { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var r = CommandLine.Parser.Default
            .ParseArguments<UplinkOptions>(args)
            .MapResult(
                (UplinkOptions o) => Launch(o),
                fail => -1
                );
        }

        private static int Launch(IOptions options)
        {
            var p = new Program();

            var director = new TestDirector(
                new WorkerSerialTransport<CommandJsonSerializer>(options.Port, options.BaudRate));

            if (options is UplinkOptions)
            {
                p.Uplink(director, options as UplinkOptions);
            }

            return 0;
        }

        private void Uplink(TestDirector director, UplinkOptions options)
        {
            Console.WriteLine($"Uplink {options.Source}");

            director.UplinkTestAssembly(options.Source);
        }
    }
}
