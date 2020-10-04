﻿using System;
using System.IO;
using System.Text;

namespace Meadow.TestsSuite
{
    public class CommandJsonSerializer : ICommandSerializer
    {
        public TestCommand Deserialize(ReadOnlySpan<byte> commandPayload)
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

        public byte[] Serialize(TestCommand command)
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
    }
}