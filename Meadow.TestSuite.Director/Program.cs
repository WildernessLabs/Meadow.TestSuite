using CommandLine;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TestSuite.Unit.Tests")]

namespace Meadow.TestSuite
{
    // TEST command lines
    //
    // uplink -p COM12 -s "..\..\..\..\Tests.Meadow.Core\bin\Debug\net472\Tests.Meadow.Core.dll"
    // assembly -l -p COM12
    // test -l -p COM12
    // test -p COM12 -e Tests.Meadow.Core.GpioTests.LedTest

    class Program
    {
        static void Main(string[] args)
        {
            var r = CommandLine.Parser.Default
            .ParseArguments<UplinkOptions, AssemblyOptions, TestOptions>(args)
            .MapResult(
                (UplinkOptions o) => Launch(o),
                (AssemblyOptions o) => Launch(o),
                (TestOptions o) => Launch(o),
                fail => -1
                );
        }

        private static int Launch(IOptions options)
        {
            var p = new Program();

            var serializer = new CommandJsonSerializer();
            serializer.UseLibrary = JsonLibrary.SystemTextJson;

            var director = new TestDirector(
                serializer,
                new WorkerSerialTransport(serializer, options.Port, options.BaudRate)
                );

            if (options is UplinkOptions)
            {
                p.Uplink(director, options as UplinkOptions);
            }
            else if (options is AssemblyOptions)
            {
                p.ProcessAssemblyCommand(director, options as AssemblyOptions);
            }
            else if (options is TestOptions)
            {
                p.ProcessTestCommand(director, options as TestOptions);
            }
            return 0;
        }

        private void Uplink(TestDirector director, UplinkOptions options)
        {
            Console.WriteLine($"Uplink {options.Source}");

            director.UplinkTestAssembly(options.Source);
        }

        private void ProcessAssemblyCommand(TestDirector director, AssemblyOptions options)
        {
            Console.WriteLine($"Assembly command");

            director.GetAssemblies();
        }

        private void ProcessTestCommand(TestDirector director, TestOptions options)
        {
            Console.WriteLine($"Test command");

            if (options.List)
            {
                director.GetTestNames();
            }
            else if(!string.IsNullOrEmpty(options.Execute))
            {
                // allow a few delimiters
                var names = options.Execute.Split(new char[] { ';', ',', '|' }, StringSplitOptions.RemoveEmptyEntries);
                director.ExecuteTests(names);
            }
        }
    }
}
