using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Meadow.TestsSuite;

namespace Meadow.TestSuite
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new Program();
            
            p.Start();

            //p.UplinkAssembly(@".\\SampleAssets\\test_text.txt");
            p.UplinkAssembly(@"..\..\..\..\Tests.Meadow.Core\bin\Debug\net472\Tests.Meadow.Core.dll");
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
