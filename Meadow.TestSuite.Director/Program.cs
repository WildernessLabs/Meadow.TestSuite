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

            var result = director.UplinkTestAssembly(options.Source);
            Console.WriteLine($"  {result}");
        }

        private void ProcessAssemblyCommand(TestDirector director, AssemblyOptions options)
        {
            // we only have one assembly command right now
            Console.WriteLine($"List of Assemblies:");

            var result = director.GetAssemblies();
            if((result == null) || (result.Length == 0))
            {
                Console.WriteLine($"  <none>");
            }
            else
            {
                foreach(var a in result)
                {
                    Console.WriteLine($"  {a}");
                }
            }
        }

        private void ProcessTestCommand(TestDirector director, TestOptions options)
        {
            if (options.List)
            {
                Console.WriteLine($"List of Tests:");

                var result = director.GetTestNames();

                if ((result == null) || (result.Length == 0))
                {
                    Console.WriteLine($"  <none>");
                }
                else
                {
                    foreach (var t in result)
                    {
                        Console.WriteLine($"  {t}");
                    }
                }
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
