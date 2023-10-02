using Meadow;
using Meadow.Validation;

public class MeadowF7TestDevice : MeadowTestDevice
{
    public new IF7MeadowDevice Device { get; }

    public MeadowF7TestDevice(IF7MeadowDevice device)
        : base(device)
    {
        Device = device;
    }
}