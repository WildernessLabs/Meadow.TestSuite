using Meadow;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;
using Meadow.Peripherals.Sensors.Buttons;
using Meadow.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Validation;

public class DisplayService
{
    public event EventHandler<TestResult>? ResultClicked;
    public event EventHandler? PublishClicked;

    private AbsoluteLayout _testLayout;
    private Label _testNameLabel;
    private Label _questionLabel;
    private Label _instructionLabel;
    private Label _inputsLabel;
    private Button _yesButton;
    private Button _noButton;
    private Button _skipButton;

    private AbsoluteLayout _resultsLayout;
    private Label _resultPassLabel;
    private Label _resultFailLabel;
    private Label _resultSkipLabel;
    private Button _publishButton;

    private AbsoluteLayout _statusLayout;
    private Label _statusLabel;

    public DisplayScreen Screen { get; }

    public DisplayService(
        DisplayScreen screen)
    {
        Screen = screen;
    }

    public void CreateValidationControls(
        IButton? yesButton = null,
        IButton? noButton = null,
        IButton? skipButton = null)
    {
        Screen.Controls.Clear();
        Screen.BackgroundColor = Color.Black;

        var buttonFont = new Font16x24();

        _testLayout = new AbsoluteLayout(0, 0, Screen.Width, Screen.Height);
        _testNameLabel = new Label(0, 0, Screen.Width, buttonFont.Height + 4)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            Font = new Font12x16(),
            TextColor = Color.Yellow
        };

        _questionLabel = new Label(0, buttonFont.Height * 2, Screen.Width, buttonFont.Height + 4)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            Font = new Font8x12(),
            TextColor = Color.White
        };

        _instructionLabel = new Label(0, buttonFont.Height * 3, Screen.Width, buttonFont.Height + 4)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            Font = new Font8x12(),
            TextColor = Color.White
        };

        _inputsLabel = new Label(0, buttonFont.Height * 4, Screen.Width, buttonFont.Height + 4)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            Font = new Font8x12(),
            TextColor = Color.Yellow
        };

        var buttonHeight = 50;
        var buttonWidth = 100;

        _yesButton = new Button(
            0,
            Screen.Height - buttonHeight - 2,
            buttonWidth,
            buttonHeight)
        {
            Font = buttonFont,
            Text = "YES",
            HighlightColor = Color.White,
            ForeColor = Color.LightGray,
            PressedColor = Color.DarkGray
        };
        _yesButton.Clicked += (s, e) => { ResultClicked?.Invoke(null, TestResult.Pass); };

        _skipButton = new Button(
            (Screen.Width - buttonWidth) / 2,
            Screen.Height - buttonHeight - 2,
            buttonWidth,
            buttonHeight)
        {
            Font = buttonFont,
            Text = "Skip",
            HighlightColor = Color.White,
            ForeColor = Color.LightGray,
            PressedColor = Color.DarkGray
        };
        _skipButton.Clicked += (s, e) => { ResultClicked?.Invoke(null, TestResult.Skip); };

        _noButton = new Button(
            Screen.Width - buttonWidth - 2,
            Screen.Height - buttonHeight - 2,
            buttonWidth,
            buttonHeight)
        {
            Font = buttonFont,
            Text = "No",
            HighlightColor = Color.White,
            ForeColor = Color.LightGray,
            PressedColor = Color.DarkGray
        };
        _noButton.Clicked += (s, e) => { ResultClicked?.Invoke(null, TestResult.Fail); };
        _testLayout.Controls.Add(_testNameLabel, _questionLabel, _instructionLabel, _inputsLabel, _yesButton, _noButton, _skipButton);

        _resultsLayout = new AbsoluteLayout(0, 0, Screen.Width, Screen.Height)
        {
            IsVisible = false
        };

        _resultPassLabel = new Label(0, 0, Screen.Width, buttonFont.Height)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            Font = new Font12x16(),
            TextColor = Color.Green,
            Text = "[N] passed"
        };
        _resultFailLabel = new Label(0, _resultPassLabel.Bottom + 2, Screen.Width, buttonFont.Height)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            Font = new Font12x16(),
            TextColor = Color.Red,
            Text = "[N] failed"
        };
        _resultSkipLabel = new Label(0, _resultFailLabel.Bottom + 2, Screen.Width, buttonFont.Height)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            Font = new Font12x16(),
            TextColor = Color.Yellow,
            Text = "[N] skipped"
        };
        _publishButton = new Button(
            (Screen.Width - buttonWidth) / 2,
            Screen.Height - buttonHeight - 2,
            200,
            buttonHeight)
        {
            Font = buttonFont,
            Text = "Publish",
            HighlightColor = Color.White,
            ForeColor = Color.LightGray,
            PressedColor = Color.DarkGray
        };
        _publishButton.Clicked += (s, e) => { PublishClicked?.Invoke(null, EventArgs.Empty); };

        _resultsLayout.Controls.Add(_resultPassLabel, _resultFailLabel, _resultSkipLabel, _publishButton);


        if (yesButton != null)
        {
            yesButton.Clicked += (s, e) => _yesButton.Click();
        }
        if (noButton != null)
        {
            noButton.Clicked += (s, e) => _noButton.Click();
        }
        if (skipButton != null)
        {
            skipButton.Clicked += (s, e) =>
            {
                _skipButton.Click();
                _publishButton.Click();
            };
        }

        _statusLayout = new AbsoluteLayout(0, 0, Screen.Width, Screen.Height)
        {
            IsVisible = false
        };
        _statusLabel = new Label(0, 0, Screen.Width, Screen.Height)
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Font = new Font12x16(),
            TextColor = Color.White
        };
        _statusLayout.Controls.Add(_statusLabel);

        Screen.Controls.Add(_testLayout, _resultsLayout, _statusLayout);

        _resultsLayout.IsVisible = false;
        _statusLayout.IsVisible = false;
    }

    public void ShowResults(IEnumerable<TestInfo> tests)
    {
        var passCount = tests.Count(t => t.Result == TestResult.Pass);
        var failCount = tests.Count(t => t.Result == TestResult.Fail);
        var skipCount = tests.Count(t => t.Result == TestResult.Skip);

        _resultPassLabel.Text = $"{passCount} passed";
        _resultFailLabel.Text = $"{failCount} failed";
        _resultSkipLabel.Text = $"{skipCount} skipped";

        _testLayout.IsVisible = false;
        _statusLayout.IsVisible = false;
        _resultsLayout.IsVisible = true;
    }

    public void ClearTestLabels()
    {
        _testNameLabel.Text = string.Empty;
        _questionLabel.Text = string.Empty;
        _instructionLabel.Text = string.Empty;
        _inputsLabel.Text = string.Empty;
    }

    public void SetTestName(string name)
    {
        if (!_testLayout.IsVisible)
        {
            _testLayout.IsVisible = true;
            _statusLayout.IsVisible = false;
        }
        _testNameLabel.Text = name;
    }

    public void SetInstructionText(string text)
    {
        _instructionLabel.Text = text;
    }

    public void SetQuestionText(string text)
    {
        _questionLabel.Text = text;
    }

    public void SetInputsLabel(string text)
    {
        _inputsLabel.Text = text;
    }

    public void ShowStatus(string text)
    {
        _statusLabel.Text = text;

        _testLayout.IsVisible = false;
        _resultsLayout.IsVisible = false;
        _statusLayout.IsVisible = true;
    }

    internal void HidePublishButton()
    {
        _publishButton.IsVisible = false;
    }
}
