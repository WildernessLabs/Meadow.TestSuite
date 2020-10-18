using System;

namespace Meadow.TestSuite
{
    public interface ICommandSerializer
    {
        TestCommand Deserialize(ReadOnlySpan<byte> commandPayload);
        T Deserialize<T>(ReadOnlySpan<byte> commandPayload);
        byte[] SerializeCommand(TestCommand command);
        byte[] SerializeResult(object result);
    }
}