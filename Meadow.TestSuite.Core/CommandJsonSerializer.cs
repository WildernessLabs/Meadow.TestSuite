using System;
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

        private TestCommand DeserializeSystemTextJson(ReadOnlySpan<byte> commandPayload)
        {
            try
            {                
                var json = Encoding.ASCII.GetString(commandPayload.ToArray());
                var command = System.Text.Json.JsonSerializer.Deserialize<TestCommand>(json);

                switch (command.CommandType)
                {
                    case CommandType.UplinkFile:
                        Console.WriteLine($" Uplink File received.  Extracting payload...");

                        var ufc = System.Text.Json.JsonSerializer.Deserialize<UplinkFileCommand>(json);
                        ProcessCommand(ufc);
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
            try
            {
                var json = Encoding.ASCII.GetString(commandPayload.ToArray());
                var command = SimpleJson.SimpleJson.DeserializeObject<TestCommand>(json);

                switch(command.CommandType)
                {
                    case CommandType.UplinkFile:
                        Console.WriteLine($" Uplink File received.  Extracting payload...");
                        var ufc = SimpleJson.SimpleJson.DeserializeObject<UplinkFileCommand>(json);
                        ProcessCommand(ufc);
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

        private void ProcessCommand(TestCommand command)
        {
            // TODO: push the processing to the actual command instance
            if(command is UplinkFileCommand)
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
                        if(fi.Exists)
                        {
                            Console.WriteLine("Destination file exists.  Overwriting.");
                        }

                        File.WriteAllBytes(ufc.Destination, data);

                        fi.Refresh();

                        Console.WriteLine($"Destination file verified to be {fi.Length} bytes.");
                    }
                }
            }
        }


        private TestCommand DeserializeNewtonsoft(ReadOnlySpan<byte> commandPayload)
        {
            try
            {
                var json = Encoding.ASCII.GetString(commandPayload.ToArray());
                var command = Newtonsoft.Json.JsonConvert.DeserializeObject<TestCommand>(json);

                switch (command.CommandType)
                {
                    case CommandType.UplinkFile:
                        Console.WriteLine($" Uplink File received.  Extracting payload...");
                        var ufc = Newtonsoft.Json.JsonConvert.DeserializeObject<UplinkFileCommand>(json);
                        ProcessCommand(ufc);
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
                    var json = System.Text.Json.JsonSerializer.Serialize(command, command.GetType());
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