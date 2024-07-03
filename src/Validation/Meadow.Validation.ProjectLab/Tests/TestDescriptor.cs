using Meadow.Devices;
using System;

namespace Validation;

public abstract class TestDescriptor
{
    public event EventHandler<string>? UpdateInputs;

    public IProjectLabHardware Hardware { get; }
    public string TestName { get; }
    public string QuestionText { get; }
    protected bool TestComplete { get; private set; } = false;

    public TestDescriptor(IProjectLabHardware hardware, string testName, string questionText)
    {
        Hardware = hardware;
        TestName = testName;
        QuestionText = questionText;
    }

    public void ResultReceived()
    {
        TestComplete = true;
    }

    public void RaiseUpdateInputs(string inputs)
    {
        UpdateInputs?.Invoke(this, inputs);
    }

    public abstract void BeginTest();
}
