using Meadow;

namespace ProjectLabBaseTests;

public abstract class TestBase : IDeviceTest
{
    public abstract bool Execute(IMeadowDevice device);
    public virtual string Name => GetType().Name.Replace("Test", string.Empty);

    protected void ReportInfo(string message)
    {
        Resolver.Log.Info($">>> {message} <<<");
    }
}
