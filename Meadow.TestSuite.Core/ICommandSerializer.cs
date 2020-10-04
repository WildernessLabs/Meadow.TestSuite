using System;

namespace Meadow.TestsSuite
{
    public interface ICommandSerializer
    {
        TestCommand Deserialize(ReadOnlySpan<byte> commandPayload);
        byte[] Serialize(TestCommand command);
    }
}