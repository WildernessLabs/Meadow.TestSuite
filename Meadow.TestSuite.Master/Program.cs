using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Meadow.TestsSuite;

namespace Meadow.TestSuite
{
    class TestWorker
    {
        public TestDirector Director { get; }

        public TestWorker()
        {

            //            Director.Registry.RegisterTest("TestA", "general", TestA);
            //            Director.Registry.RegisterTest("TestA", "other", TestB);
            //            Director.Registry.RegisterTest("TestA", "general", TestC);
        }

        TestResult TestA()
        {
            var result = new TestResult();

            result.State = TestState.Success;
            result.Output.Add("A");
            Thread.Sleep(100);
            return result;
        }

        TestResult TestB()
        {
            var result = new TestResult();

            result.State = TestState.Success;
            result.Output.Add("B");
            Thread.Sleep(150);
            return result;
        }

        TestResult TestC()
        {
            var result = new TestResult();

            result.State = TestState.Failed;
            result.Output.Add("C");
            Thread.Sleep(100);
            return result;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var p = new Program();
            
            p.Start();

            //p.UplinkAssembly(@"C:\Repos\WildernessLabs\Meadow.TestSuite\Tests.Meadow.Core\bin\Debug\net472\Tests.Meadow.Core.dll");
            p.UplinkAssembly(@".\\SampleAssets\\test_text.txt");
        }

        public TestDirector Director { get; private set; }

        private void Start()
        {
            Console.WriteLine("Hello Test!");

            Director = new TestDirector(
                new WorkerSerialTransport<CommandJsonSerializer>("COM12", 9600));
        }

        public void UplinkAssembly(string assemblyPath)
        {
            Director.UplinkTestAssembly(assemblyPath);
        }
    }
}
