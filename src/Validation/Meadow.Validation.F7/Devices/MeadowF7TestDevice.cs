using Meadow;
using Meadow.Validation;

public class MeadowF7TestDevice : IDeviceUnderTest<IF7MeadowDevice>
{
    public IF7MeadowDevice Device { get; }

    public MeadowF7TestDevice(IF7MeadowDevice device)
    {
        Device = device;
    }
}