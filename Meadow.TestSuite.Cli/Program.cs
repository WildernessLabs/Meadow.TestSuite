using CommandLine;
using System;
using System.Net;

namespace Meadow.TestSuite.Cli
{
    // TEST command lines
    //
    // uplink -p COM12 -s "..\..\..\..\Tests.Meadow.Core\bin\Debug\net472\Tests.Meadow.Core.dll"
    // uplink -e 192.168.10.123:8000 -s "..\..\..\..\Tests.Meadow.Core\bin\Debug\net472\Tests.Meadow.Core.dll"
    // assembly -l -p COM12
    // test -l -p COM12
    // test -p COM12 -e Tests.Meadow.Core.GpioTests.LedTest
    // result --all -p COM12

    class Program
    {
        static void Main(string[] args)
        {
            var r = CommandLine.Parser.Default
            .ParseArguments<UplinkOptions, AssemblyOptions, TestOptions, ResultOptions>(args)
            .MapResult(
                (UplinkOptions o) => Launch(o),
                (AssemblyOptions o) => Launch(o),
                (TestOptions o) => Launch(o),
                (ResultOptions o) => Launch(o),
                fail => -1
                );
        }

        private static ITestDirector GetDirector(IOptions options, ICommandSerializer serializer)
        {
            // are we using rest or serial?
            if (!string.IsNullOrEmpty(options.Ethernet))
            {
                if (!IPEndPoint.TryParse(options.Ethernet, out var ep))
                {
                    Console.WriteLine($"Invalid IP End Point '{options.Ethernet}'");
                    return null;
                }
                return new RestTestDirector(ep);
            }
            else if (!string.IsNullOrEmpty(options.Port))
            {
                var transport = new WorkerSerialTransport(serializer, options.Port, options.BaudRate);
                return new SerialTestDirector(serializer, transport);
            }
            else
            {
                Console.WriteLine($"Either a Serial Port or IP End Point is required");
                return null;
            }
        }

        private static int Launch(IOptions options)
        {
            var p = new Program();

            var serializer = new CommandJsonSerializer();
            serializer.UseLibrary = JsonLibrary.SystemTextJson;

            var director = GetDirector(options, serializer);
            if (director == null)
            {
                Console.WriteLine($"No transport to Worker defined");
                return -1;
            }

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
            else if (options is ResultOptions)
            {
                p.ProcessResultCommand(director, options as ResultOptions);
            }
            return 0;
        }

        private void Uplink(ITestDirector director, UplinkOptions options)
        {
            Console.WriteLine($"Uplink {options.Source}");

            var task = director.SendFile(new System.IO.FileInfo( options.Source), options.Destination);
            task.Wait();
            Console.WriteLine($"  sent.");
        }

        private void ProcessAssemblyCommand(ITestDirector director, AssemblyOptions options)
        {
            if (options.List)
            {
                Console.WriteLine($"List of Assemblies:");
                var task = director.GetAssemblies();
                task.Wait();
                if ((task.Result == null) || (task.Result.Length == 0))
                {
                    Console.WriteLine($"  <none>");
                }
                else
                {
                    foreach (var a in task.Result)
                    {
                        Console.WriteLine($"  {a}");
                    }
                }
            }
            else if (options.Clear)
            {
                Console.WriteLine($"Delete all Test Assemblies...");
                var result = director.DeleteAssemblies();
                Console.WriteLine(result);
            }
        }

        private void ProcessTestCommand(ITestDirector director, TestOptions options)
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
            else if (!string.IsNullOrEmpty(options.Execute))
            {
                // allow a few delimiters
                var names = options.Execute.Split(new char[] { ';', ',', '|' }, StringSplitOptions.RemoveEmptyEntries);

                TestResult[] results;

                try
                {
                    results = director.ExecuteTests(names);
                }
                catch (System.IO.FileNotFoundException fe)
                {
                    Console.WriteLine($"Unable to open the serial port: {fe.Message}");
                    return;
                }

                if (results != null)
                {
                    Console.WriteLine("Executing tests:");

                    // TODO: support verbose
                    foreach (var r in results)
                    {
                        Console.WriteLine($"  {r.TestID} as {r.ResultID}");
                    }
                }
                else
                {
                    Console.WriteLine("No results received.");
                }
            }
        }

        private void ProcessResultCommand(ITestDirector director, ResultOptions options)
        {
            TestResult[] results;

            Console.WriteLine($"List of Results:");

            if (!options.ResultID.Equals(Guid.Empty))
            {
                results = director.GetTestResults(options.ResultID);
            }
            else if (!string.IsNullOrEmpty(options.TestID))
            {
                results = director.GetTestResults(options.TestID);
            }
            else
            {
                results = director.GetTestResults();
            }

            if ((results == null) || (results.Length == 0))
            {
                Console.WriteLine($"  <none>");
            }
            else
            {
                // TODO: support verbose
                foreach (var r in results)
                {
                    Console.WriteLine($"  ({r.State})\t{r.TestID}\t{string.Join(',', r.Output)}");
                }
            }
        }
    }
}