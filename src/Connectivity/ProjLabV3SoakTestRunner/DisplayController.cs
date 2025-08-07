using Meadow;
using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;
using Meadow.Peripherals.Displays;
using System;

namespace ProjectLabTest.Services;

internal class DisplayController
{
    int _titleHeight = 24;

    int _rowHeight = 10;

    int _rowMargin = 5;

    Color _backgroundColor = Color.Black;

    Color _foregroundColor = Color.White;

    Font12x20 _titleFont = new Font12x20();

    Font6x8 _textFont = new Font6x8();

    int _currentLine = 0;

    protected DisplayScreen DisplayScreen { get; set; }

    protected Label Title { get; set; }

    protected Label Footer { get; set; }

    protected Label[] Lines { get; set; }

    public DisplayController(IPixelDisplay display)
    {
        DisplayScreen = new DisplayScreen(display, RotationType._270Degrees)
        {
            BackgroundColor = _backgroundColor
        };

        Title = new Label(_rowMargin, 0, DisplayScreen.Width - (2 * _rowMargin), _titleHeight)
        {
            Text = "",
            TextColor = _backgroundColor,
            BackgroundColor = _foregroundColor,
            Font = _titleFont,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        DisplayScreen.Controls.Add(Title);

        Footer = new Label(_rowMargin, DisplayScreen.Height - _titleHeight, DisplayScreen.Width - (2 * _rowMargin), _titleHeight)
        {
            Text = "",
            TextColor = _backgroundColor,
            BackgroundColor = _foregroundColor,
            Font = _titleFont,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        DisplayScreen.Controls.Add(Footer);

        Lines = new Label[19];
        for (int index = 0; index < Lines.Length; index++)
        {
            Lines[index] = new Label(_rowMargin, _titleHeight + (index * _rowHeight) + 1, DisplayScreen.Width - (2 * _rowMargin), _rowHeight)
            {
                Text = "",
                TextColor = _foregroundColor,
                Font = _textFont,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            DisplayScreen.Controls.Add(Lines[index]);
        }
    }

    /// <summary>
    /// Clear the lines of text from the display.  The title will be left alone.
    /// </summary>
    public void ClearText()
    {
        DisplayScreen.BeginUpdate();

        for (int index = 0; index < Lines.Length; index++)
        {
            Lines[index].Text = "";
        }

        DisplayScreen.EndUpdate();
    }

    /// <summary>
    /// Add a line of text to the bottom of the displayed lines.  The display will
    /// be scrolled up one line if the bottom line is already full.
    /// </summary>
    /// <param name="text">Line of text to add to the displayed text.</param>
    public void AddText(string text)
    {
        DisplayScreen.BeginUpdate();

        if (_currentLine == Lines.Length)
        {
            for (int index = 1; index < Lines.Length; index++)
            {
                Lines[index - 1].Text = Lines[index].Text;
            }
            Lines[_currentLine - 1].Text = text;
        }
        else
        {
            Lines[_currentLine].Text = text;
            _currentLine++;
        }

        DisplayScreen.EndUpdate();
    }

    /// <summary>
    /// Update the title line on the display.
    /// </summary>
    /// <param name="title">Title text.</param>
    public void UpdateTitle(string title)
    {
        DisplayScreen.BeginUpdate();

        Title.Text = title;

        DisplayScreen.EndUpdate();
    }

    /// <summary>
    /// Update the footer line on the display.
    /// </summary>
    /// <param name="footer">Footer text.</param>
    public void UpdateFooter(string footer)
    {
        DisplayScreen.BeginUpdate();

        Footer.Text = footer;

        DisplayScreen.EndUpdate();
    }

    /// <summary>
    /// Update the display with the specified lines.
    /// </summary>
    /// <param name="lines">Lines of text to display.</param>
    public void UpdateText(string[] lines)
    {
        DisplayScreen.BeginUpdate();

        if (lines.Length > Lines.Length)
        {
            throw new ArgumentException($"Too many lines of text.  Maximum is {Lines.Length}.");
        }

        for (int index = 0; index < Lines.Length; index++)
        {
            Lines[index].Text = lines[index];
        }

        DisplayScreen.EndUpdate();
    }
    
    /// <summary>
    /// Show the message passed in on the display with a time stamp.
    /// </summary>
    /// <param name="message">Message to be shown.</param>
    public void Log(string message, bool addTimestamp = true)
    {
        string output = addTimestamp ? $"{DateTime.Now:HH:mm:ss} {message}" : message;
        if (DisplayScreen != null)
        {
            AddText(output);
        }
        else
        {
            ConsoleLog(output);
        }
    }
    
    /// <summary>
    /// Send a message to the console.
    /// </summary>
    /// <remarks>
    /// This method is provided for when the display is not available.
    /// </remarks>
    /// <param name="message">Message to display.</param>
    private void ConsoleLog(string message)
    {
        Console.WriteLine(message);
    }
}
