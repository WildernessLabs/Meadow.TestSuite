using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Meadow.TestSuite
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
                        return System.Text.Json.JsonSerializer.Deserialize<UplinkFileCommand>(json);
                    case CommandType.EnumerateAssemblies:
                        return System.Text.Json.JsonSerializer.Deserialize<GetAssembliesCommand>(json);
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
                        return SimpleJson.SimpleJson.DeserializeObject<UplinkFileCommand>(json);
                    case CommandType.EnumerateAssemblies:
                        return SimpleJson.SimpleJson.DeserializeObject<GetAssembliesCommand>(json);
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
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<UplinkFileCommand>(json);
                    case CommandType.EnumerateAssemblies:
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<GetAssembliesCommand>(json);
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

        public byte[] SerializeCommand(TestCommand command)
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

        public byte[] SerializeResult(object result)
        {
            switch (UseLibrary)
            {
                case JsonLibrary.SimpleJson:
                    return SerializeSimpleJson(result);
                case JsonLibrary.JsonDotNet:
                    return SerializeNewtonsoft(result);
                default:
                    return SerializeSystemTextJson(result);
            }
        }

        private byte[] SerializeSimpleJson(object payload)
        {
            Console.WriteLine($" {this.GetType().Name} Deserializing...");

            try
            {
                using (var stream = new MemoryStream())
                {
                    var json = SimpleJson.SimpleJson.SerializeObject(payload);
                    return Encoding.UTF8.GetBytes(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Failed to serialize command: {ex.Message}");
            }

            return null;
        }

        private byte[] SerializeNewtonsoft(object payload)
        {
            Console.WriteLine($" {this.GetType().Name} Deserializing...");

            try
            {
                using (var stream = new MemoryStream())
                {
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
                    return Encoding.UTF8.GetBytes(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Failed to serialize command: {ex.Message}");
            }

            return null;
        }

        private byte[] SerializeSystemTextJson(object payload)
        {
            Console.WriteLine($" {this.GetType().Name} serializing...");

            try
            {
                using (var stream = new MemoryStream())
                {
                    var json = System.Text.Json.JsonSerializer.Serialize(payload, payload.GetType());
                    var bytes = Encoding.UTF8.GetBytes(json);
                    Console.WriteLine($" Serialized {bytes.Length} bytes");
                    return bytes;
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