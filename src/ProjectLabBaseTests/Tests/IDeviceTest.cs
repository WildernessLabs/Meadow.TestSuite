using Meadow;

namespace ProjectLabBaseTests;

public interface IDeviceTest
{
    public string Name { get; }
    public bool Execute(IMeadowDevice device);
}
