using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow.Validation
{
    public abstract class ValidationApp<THardware> : App<THardware>
        where THardware : class, IMeadowDevice
    {
        public abstract IEnumerable<ITest<MeadowTestDevice>> TestsToRun { get; }
        public abstract MeadowTestDevice DeviceUnderTest { get; }

        public abstract void DisplayTestsRunning();
        public abstract void DisplaySuccess();
        public abstract void DisplayFailure();
        public virtual void OnExecutionHeartbeat() { }

        public override async Task Run()
        {
            DisplayTestsRunning();

            Resolver.Log.Info($"Starting validation tests...");

            var failed = new List<string>();
            var complete = false;
            var success = true;

            new Thread(() =>
            {
                while (!complete)
                {
                    OnExecutionHeartbeat();

                    Thread.Sleep(1000);
                }

            }).Start();

            try
            {
                foreach (var test in TestsToRun)
                {
                    Resolver.Log.Info($"Running {test.GetType().Name}...");

                    var result = await test.RunTest(DeviceUnderTest);

                    if (!result)
                    {
                        failed.Add(test.GetType().Name);
                    }

                    success &= result;
                }

                Resolver.Log.Info($"Tests complete.");
            }
            finally
            {
                complete = true;
            }

            if (success)
            {
                DisplaySuccess();
            }
            else
            {
                DisplayFailure();
                Resolver.Log.Error("---- FAILED TESTS----");
                Resolver.Log.Error(string.Join("\r\n ", failed));
            }
        }
    }
}