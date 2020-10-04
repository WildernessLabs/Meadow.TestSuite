using System;
using System.IO;
using System.Text;

namespace Meadow.TestsSuite
{
    public class CommandJsonSerializer : ICommandSerializer
    {
        public enum JsonLibrary
        {
            SystemTextJson,
            SimpleJson,
            JsonDotNet
        }

        public JsonLibrary UseLibrary { get; set; } = CommandJsonSerializer.JsonLibrary.SimpleJson;

        public TestCommand Deserialize(ReadOnlySpan<byte> commandPayload)
        {
            switch(UseLibrary)
            {
                case JsonLibrary.SimpleJson:
                    return DeserializeSimpleJson(commandPayload);
                case JsonLibrary.JsonDotNet:
                    return DeserializeNewtonsoft(commandPayload);
                default:
                    return DeserializeSystemTextJson(commandPayload);
            }
        }

        private TestCommand DeserializeSystemTextJson(ReadOnlySpan<byte> commandPayload)
        {
            Console.WriteLine($" {this.GetType().Name} Deserializing...");

            try
            {
                
                var json = Encoding.ASCII.GetString(commandPayload.ToArray());
                var command = System.Text.Json.JsonSerializer.Deserialize<TestCommand>(json);

                switch (command.CommandType)
                {
                    case CommandType.UplinkFile:
                        Console.WriteLine($" Uplink File received.  Extracting payload...");
                        var ufc = System.Text.Json.JsonSerializer.Deserialize<UplinkFileCommand>(json);
                        Console.WriteLine($" File: {ufc.Destination}");
                        var di = new DirectoryInfo(Path.GetDirectoryName(ufc.Destination));
                        if (!di.Exists)
                        {
                            Console.WriteLine($" Creating directory {di.FullName}");
                            di.Create();
                        }
                        var data = Convert.FromBase64String(ufc.FileData);
                        File.WriteAllBytes(ufc.Destination, data);
                        break;
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
            Console.WriteLine($" {this.GetType().Name} Deserializing...");

            try
            {
                var json = Encoding.ASCII.GetString(commandPayload.ToArray());
                var command = SimpleJson.SimpleJson.DeserializeObject<TestCommand>(json);

                switch(command.CommandType)
                {
                    case CommandType.UplinkFile:
                        Console.WriteLine($" Uplink File received.  Extracting payload...");
                        var ufc = SimpleJson.SimpleJson.DeserializeObject<UplinkFileCommand>(json);
                        Console.WriteLine($" File: {ufc.Destination}");
                        var di = new DirectoryInfo(Path.GetDirectoryName(ufc.Destination));
                        if(!di.Exists)
                        {
                            Console.WriteLine($" Creating directory {di.FullName}");
                            di.Create();
                        }
                        var data = Convert.FromBase64String(ufc.FileData);
                        File.WriteAllBytes(ufc.Destination, data);
                    break;
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
            Console.WriteLine($" {this.GetType().Name} Deserializing...");

            try
            {
                var json = Encoding.ASCII.GetString(commandPayload.ToArray());
                var command = Newtonsoft.Json.JsonConvert.DeserializeObject<TestCommand>(json);

                switch (command.CommandType)
                {
                    case CommandType.UplinkFile:
                        Console.WriteLine($" Uplink File received.  Extracting payload...");
                        var ufc = Newtonsoft.Json.JsonConvert.DeserializeObject<UplinkFileCommand>(json);
                        Console.WriteLine($" File: {ufc.Destination}");
                        var di = new DirectoryInfo(Path.GetDirectoryName(ufc.Destination));
                        if (!di.Exists)
                        {
                            Console.WriteLine($" Creating directory {di.FullName}");
                            di.Create();
                        }
                        var data = Convert.FromBase64String(ufc.FileData);
                        File.WriteAllBytes(ufc.Destination, data);
                        break;
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
                    return Encoding.ASCII.GetBytes(json);
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
                    return Encoding.ASCII.GetBytes(json);
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
                    var json = System.Text.Json.JsonSerializer.Serialize(command);
                    return Encoding.ASCII.GetBytes(json);
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