using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Meadow.TestsSuite
{
    public enum JsonLibrary
    {
        SystemTextJson,
        SimpleJson,
        JsonDotNet
    }

    public class CommandJsonSerializer : ICommandSerializer
    {
        public JsonLibrary UseLibrary { get; set; } = JsonLibrary.JsonDotNet;

        public TestCommand Deserialize(ReadOnlySpan<byte> commandPayload)
        {
            Console.WriteLine($" {this.GetType().Name} Deserializing with {this.UseLibrary}...");

            var sw = new Stopwatch();

            try
            {
                sw.Start();
                switch (UseLibrary)
                {
                    case JsonLibrary.SimpleJson:
                        return DeserializeSimpleJson(commandPayload);
                    case JsonLibrary.JsonDotNet:
                        return DeserializeNewtonsoft(commandPayload);
                    default:
                        return DeserializeSystemTextJson(commandPayload);
                }
            }
            finally
            {
                sw.Stop();
                Console.WriteLine($" Deserilization took {sw.ElapsedMilliseconds}ms");
            }
        }

        private TestCommand DeserializeSystemTextJson(ReadOnlySpan<byte> commandPayload)
        {
            try
            {                
                var json = Encoding.UTF8.GetString(commandPayload.ToArray());
                var command = System.Text.Json.JsonSerializer.Deserialize<TestCommand>(json);

                switch (command.CommandType)
                {
                    case CommandType.UplinkFile:
                        Console.WriteLine($" Uplink File received.  Extracting payload...");

                        return System.Text.Json.JsonSerializer.Deserialize<UplinkFileCommand>(json);
                    default:
                        Console.WriteLine($" Command '{command.CommandType}' not yet implemented");
                        break;
                }

                return command;
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Failed to deserialize command: {ex.Message}");
            }

            return null;
        }

        private TestCommand DeserializeSimpleJson(ReadOnlySpan<byte> commandPayload)
        {
            try
            {
                var json = Encoding.UTF8.GetString(commandPayload.ToArray());
                var command = SimpleJson.SimpleJson.DeserializeObject<TestCommand>(json);

                switch(command.CommandType)
                {
                    case CommandType.UplinkFile:
                        Console.WriteLine($" Uplink File received.  Extracting payload...");
                        return SimpleJson.SimpleJson.DeserializeObject<UplinkFileCommand>(json);
                    default:
                        Console.WriteLine($" Command '{command.CommandType}' not yet implemented");
                        break;
                }

                return command;
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Failed to deserialize command: {ex.Message}");
            }

            return null;
        }

        private TestCommand DeserializeNewtonsoft(ReadOnlySpan<byte> commandPayload)
        {
            try
            {
                var json = Encoding.UTF8.GetString(commandPayload.ToArray());
                var command = Newtonsoft.Json.JsonConvert.DeserializeObject<TestCommand>(json);

                switch (command.CommandType)
                {
                    case CommandType.UplinkFile:
                        Console.WriteLine($" Uplink File received.  Extracting payload...");
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<UplinkFileCommand>(json);
                    default:
                        Console.WriteLine($" Command '{command.CommandType}' not yet implemented");
                        break;
                }

                return command;
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Failed to deserialize command: {ex.Message}");
            }

            return null;
        }

        public byte[] Serialize(TestCommand command)
        {
            switch (UseLibrary)
            {
                case JsonLibrary.SimpleJson:
                    return SerializeSimpleJson(command);
                case JsonLibrary.JsonDotNet:
                    return SerializeNewtonsoft(command);
                default:
                    return SerializeSystemTextJson(command);
            }
        }

        private byte[] SerializeSimpleJson(TestCommand command)
        {
            Console.WriteLine($" {this.GetType().Name} Deserializing...");

            try
            {
                using (var stream = new MemoryStream())
                {
                    var json = SimpleJson.SimpleJson.SerializeObject(command);
                    return Encoding.UTF8.GetBytes(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Failed to serialize command: {ex.Message}");
            }

            return null;
        }

        private byte[] SerializeNewtonsoft(TestCommand command)
        {
            Console.WriteLine($" {this.GetType().Name} Deserializing...");

            try
            {
                using (var stream = new MemoryStream())
                {
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(command);
                    return Encoding.UTF8.GetBytes(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Failed to serialize command: {ex.Message}");
            }

            return null;
        }

        private byte[] SerializeSystemTextJson(TestCommand command)
        {
            Console.WriteLine($" {this.GetType().Name} Deserializing...");

            try
            {
                using (var stream = new MemoryStream())
                {
                    var json = System.Text.Json.JsonSerializer.Serialize(command, command.GetType());
                    return Encoding.UTF8.GetBytes(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Failed to serialize command: {ex.Message}");
            }

            return null;
        }
    }
}