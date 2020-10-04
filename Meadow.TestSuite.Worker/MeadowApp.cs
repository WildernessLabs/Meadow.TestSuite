using Meadow;
using Meadow.Devices;
using Meadow.TestsSuite;
using SimpleJson;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace MeadowApp
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        public MeadowApp()
        {
            Console.WriteLine("+ MeadowApp");
            
            Console.WriteLine(" Creating serial port...");
            var port = Device.CreateSerialPort(
                Device.SerialPortNames.Com4, 
                9600, 
                8, 
                Meadow.Hardware.
                Parity.None, 
                Meadow.Hardware.StopBits.One);

//            var serializer = new CommandCustomSerializer();
            var serializer = new CommandJsonSerializer();
            serializer.UseLibrary = JsonLibrary.JsonDotNet;

            var listener = new SerialListener(port, serializer);
            listener.CommandReceived += Listener_CommandReceived;
            listener.StartListening();

            Thread.Sleep(Timeout.Infinite);

            Console.WriteLine("- MeadowApp");
        }

        private void Listener_CommandReceived(TestCommand command)
        {
            // TODO: push the processing to the actual command instance or move this to some form of "processor" class
            if (command is UplinkFileCommand)
            {
                var ufc = command as UplinkFileCommand;

                if (ufc == null)
                {
                    Console.WriteLine($" Deserializer returned null.");
                }
                else
                {
                    Console.WriteLine($" Data: {ufc.FileData.Length} Base64 chars");
                    Console.WriteLine($" Destination: {ufc.Destination}");

                    if (string.IsNullOrEmpty(ufc.Destination))
                    {
                        Console.WriteLine($" Invalid/missing file destination");
                    }
                    else
                    {
                        var di = new DirectoryInfo(Path.GetDirectoryName(ufc.Destination));
                        if (!di.Exists)
                        {
                            Console.WriteLine($" Creating directory {di.FullName}");
                            di.Create();
                        }
                        var data = Convert.FromBase64String(ufc.FileData);
                        var fi = new FileInfo(ufc.Destination);
                        if (fi.Exists)
                        {
                            Console.WriteLine("Destination file exists.  Overwriting.");
                        }

                        File.WriteAllBytes(ufc.Destination, data);

                        fi.Refresh();

                        Console.WriteLine($"Destination file verified to be {fi.Length} bytes.");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Don't know how to process {command.GetType().Name}");
            }
        }

        private void LoadAndRun()
        {
            try
            {
                var name = "/meadow0/Tests.Meadow.Core.dll";

                Console.WriteLine($"Getting info for '{name}'...");
                var f = new FileInfo(name);
                if (!f.Exists)
                {
                    Console.WriteLine("FileInfo says target does not exist.");

                    Console.WriteLine($"Checking with File for '{name}'...");
                    if (!File.Exists(name))
                    {
                        Console.WriteLine("File.Exists says target does not exist.");
                        return;
                    }
                }

                Console.WriteLine($"Loading assembly '{name}'...");
                var asm = Assembly.LoadFrom(name);

                Console.WriteLine("Looking for tests...");
                var testTypes = asm.GetTypes().Where(t => typeof(IExecutableTest).IsAssignableFrom(t));

                var count = 0;

                if(testTypes == null)
                {
                    Console.WriteLine("No tests found.");
                    return;
                }
                else
                {
                    count = testTypes.Count();
                    Console.WriteLine($"{count} {(count == 1 ? "test" : "tests")} found.");
                }

                if (count > 0)
                {
                    Console.WriteLine("Running tests...");
                    foreach (var type in testTypes)
                    {                        
                        Console.WriteLine($"  Creating test {type.Name}...");
                        var instance = Activator.CreateInstance(type) as IExecutableTest;
                        Console.WriteLine($"  Running test {type.Name}...");
                        var result = instance.TestFunction();
                        Console.WriteLine($"    > {result.State}");
                    }
                }
                else
                {
                    foreach(var t in asm.GetTypes())
                    {
                        Console.WriteLine($"  Found type {t.Name}");
                        var test = t as IExecutableTest;
                        if(test == null)
                        {
                            Console.WriteLine($"   Can't cast to IExecutableTest");
                        }
                        else
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FAILED: {ex.Message}");
            }
        }

        /*
        void Initialize()
        {
            Console.WriteLine("Initialize hardware...");

            onboardLed = new RgbPwmLed(device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue,
                3.3f, 3.3f, 3.3f,
                Meadow.Peripherals.Leds.IRgbLed.CommonType.CommonAnode);
        }

        void CycleColors(int duration)
        {
            Console.WriteLine("Cycle colors...");

            while (true)
            {
                ShowColorPulse(Color.Blue, duration);
                ShowColorPulse(Color.Cyan, duration);
                ShowColorPulse(Color.Green, duration);
                ShowColorPulse(Color.GreenYellow, duration);
                ShowColorPulse(Color.Yellow, duration);
                ShowColorPulse(Color.Orange, duration);
                ShowColorPulse(Color.OrangeRed, duration);
                ShowColorPulse(Color.Red, duration);
                ShowColorPulse(Color.MediumVioletRed, duration);
                ShowColorPulse(Color.Purple, duration);
                ShowColorPulse(Color.Magenta, duration);
                ShowColorPulse(Color.Pink, duration);
            }
        }

        void ShowColorPulse(Color color, int duration = 1000)
        {
            onboardLed.StartPulse(color, (uint)(duration / 2));
            Thread.Sleep(duration);
            onboardLed.Stop();
        }

        void ShowColor(Color color, int duration = 1000)
        {
            Console.WriteLine($"Color: {color}");
            onboardLed.SetColor(color);
            Thread.Sleep(duration);
            onboardLed.Stop();
        }
        */
    }
}