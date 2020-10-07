using System;

namespace Meadow.TestSuite
{
    public interface ICommandSerializer
    {
        TestCommand Deserialize(ReadOnlySpan<byte> commandPayload);
        byte[] Serialize(TestCommand command);
    }
}