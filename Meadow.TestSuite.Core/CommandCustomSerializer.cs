using System;
using System.Text;

namespace Meadow.TestSuite
{
    public class CommandCustomSerializer : ICommandSerializer
    {
        public TestCommand Deserialize(ReadOnlySpan<byte> commandPayload)
        {
            Console.WriteLine($" {this.GetType().Name} Deserializing...");

            try
            {
                var data = Encoding.ASCII.GetString(commandPayload.ToArray());
                var i = data.IndexOf('|');
                var type = int.Parse(data.Substring(0, i));

                return new TestCommand
                { 
                    CommandType = (CommandType)type,
//                    Payload = data.Substring(i + 1)
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Failed to deserialize command: {ex.Message}");
            }

            return null;
        }

        public byte[] Serialize(TestCommand command)
        {
            Console.WriteLine(" Serializing...");

            try
            {
                var sb = new StringBuilder();
                sb.Append(command.CommandType);
                sb.Append("|");
//                sb.Append(command.Payload);
                return Encoding.ASCII.GetBytes(sb.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Failed to serialize command: {ex.Message}");
            }

            return null;
        }
    }
}