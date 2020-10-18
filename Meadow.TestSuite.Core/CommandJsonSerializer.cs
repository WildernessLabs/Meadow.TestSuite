using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
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
        public JsonLibrary UseLibrary { get; set; }
        public bool ShowDebug { get; set; } = false;

        public CommandJsonSerializer(JsonLibrary library = JsonLibrary.SystemTextJson)
        {
            UseLibrary = library;
        }

        public T Deserialize<T>(ReadOnlySpan<byte> commandPayload)
        {
            Output.WriteLineIf(ShowDebug, $" {this.GetType().Name} Deserializing with {this.UseLibrary}...");
            var sw = new Stopwatch();

            try
            {
                var json = Encoding.UTF8.GetString(commandPayload.ToArray());

                switch (UseLibrary)
                {
                    case JsonLibrary.SimpleJson:
                        return SimpleJson.SimpleJson.DeserializeObject<T>(json);
                    case JsonLibrary.JsonDotNet:
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
                    default:
                        return System.Text.Json.JsonSerializer.Deserialize<T>(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Failed to deserialize command: {ex.Message}");
                return default;
            }
            finally
            {
                sw.Stop();
                Output.WriteLineIf(ShowDebug, $" Deserilization took {sw.ElapsedMilliseconds}ms");
            }
        }

        public TestCommand Deserialize(ReadOnlySpan<byte> commandPayload)
        {
            Output.WriteLineIf(ShowDebug, $" {this.GetType().Name} Deserializing with {this.UseLibrary}...");

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
                Output.WriteLineIf(ShowDebug, $" Deserilization took {sw.ElapsedMilliseconds}ms");
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
                    case CommandType.EnumerateTests:
                        return System.Text.Json.JsonSerializer.Deserialize<GetTestNamesCommand>(json);
                    case CommandType.ExecuteTests:
                        return System.Text.Json.JsonSerializer.Deserialize<ExecuteTestsCommand>(json);
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
                    case CommandType.EnumerateTests:
                        return SimpleJson.SimpleJson.DeserializeObject<GetTestNamesCommand>(json);
                    case CommandType.ExecuteTests:
                        return SimpleJson.SimpleJson.DeserializeObject<ExecuteTestsCommand>(json);
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
                    case CommandType.EnumerateTests:
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<GetTestNamesCommand>(json);
                    case CommandType.ExecuteTests:
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<ExecuteTestsCommand>(json);
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
            Output.WriteLineIf(ShowDebug, $" {this.GetType().Name} Deserializing...");

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
                Output.WriteLineIf(ShowDebug, $" Failed to serialize command: {ex.Message}");
            }

            return null;
        }

        private byte[] SerializeSystemTextJson(object payload)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    var json = System.Text.Json.JsonSerializer.Serialize(payload, payload.GetType());
                    var bytes = Encoding.UTF8.GetBytes(json);
                    Output.WriteLineIf(ShowDebug, $" Serialized {bytes.Length} bytes");
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