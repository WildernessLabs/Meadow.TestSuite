using System;
using System.IO;

namespace Meadow.TestSuite
{
    public class CommandProtoBufSerializer : ICommandSerializer
    {
        public TestCommand Deserialize(ReadOnlySpan<byte> commandPayload)
        {
            Console.WriteLine(" Deserializing...");

            try
            {
                var command = ProtoBuf.Serializer.Deserialize<TestCommand>(commandPayload);

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
            Console.WriteLine(" Serializing...");

            try
            {
                using (var stream = new MemoryStream())
                {
                    ProtoBuf.Serializer.Serialize<TestCommand>(stream, command);
                    return stream.ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Failed to serialize command: {ex.Message}");
            }

            return null;
        }

        public byte[] SerializeResult(object result)
        {
            throw new NotImplementedException();
        }
    }
}