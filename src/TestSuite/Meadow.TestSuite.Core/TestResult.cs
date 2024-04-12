using System;
using System.Collections.Generic;

namespace Meadow.TestSuite;

public enum TestTarget
{
    MeadowF7,
    RaspberryPi
}

public class Test
{
    public string TestName { get; set; }
    public string Description { get; set; }
    public string[] SupportedTargets { get; set; }
}

public class TestResult
{
    public TestResult()
    {
    }

    public TestResult(string testID)
    {
        TestName = testID;
        State = TestState.NotRun;
    }
    /*

  {
        "TestName" : "Digital Input (Interrupts)"
		"StartedTimestamp" : "2024-04-02T16:19:23Z",
		"CompletedTimestamp" : "2024-04-02T16:20:23Z",
		"TargetPlatform" : "Meadow F7",
		"MeadowOSVersion" : "1.9.0.0",
		"TargetInfo" : "Meadow F7",
		"TestRunBy" : "ctacke",
		"Result" : "pass",
		"OuputInfo" : "ctacke tests never fail!"
	}
    */

    public Guid ResultID { get; set; }
    public string TestName { get; set; }
    public DateTime StartedTimestamp { get; set; }
    public DateTime? CompletedTimestamp { get; set; }
    public TestState State { get; set; }
    public string TargetPlatform { get; set; }
    public string MeadowOSVersion { get; set; }
    public string TargetInfo { get; set; }
    public string TestRunBy { get; set; }
    public List<string> Output { get; set; } = new List<string>();

    public override string ToString()
    {
        return $"{State}:\t{TestName}";
    }
}
