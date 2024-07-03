namespace Meadow.Validation;

public class TestInfo
{
    public TestInfo()
    {
    }

    public TestInfo(string name, TestResult result)
    {
        Name = name;
        Result = result;
    }

    public string Name { get; set; }
    public TestResult Result { get; set; }
}

public enum TestResult
{
    Pass,
    Fail,
    Skip
}
