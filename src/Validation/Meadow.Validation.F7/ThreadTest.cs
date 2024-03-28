using System;
using System.Threading.Tasks;

namespace Meadow.Validation
{
    public class ThreadTest : ITest<MeadowTestDevice>
    {
        private Random _rand = new Random();

        public async Task<bool> RunTest(MeadowTestDevice device)
        {
            Spawn("A", 2);
            Spawn("B", 1);
            Spawn("C", 3);
            Spawn("D", 3);
            Spawn("E", 3);

            while (true)
            {
                await Task.Delay(1000);
                Resolver.Log.Info("Thread M");
            }
        }

        private void Spawn(string name, int depth = 1)
        {
            Task.Run(async () =>
            {
                var d = depth - 1;

                if (d > 0)
                {
                    Spawn($"{name}_{d}", d);
                }

                while (true)
                {
                    await Task.Delay(1000);
                    Resolver.Log.Info($"Thread {name}");

                    if (_rand.Next(20) == 0)
                    {
                        Resolver.Log.Info($"Crashing {name}");
                        throw new Exception($"{name} crashed");
                    }
                }
            });
        }
    }
}
