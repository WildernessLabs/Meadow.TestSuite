using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;

using System;

namespace FeatherF7Test.Services;

internal class DisplayController
{
    /// <summary>
    /// Number of lines to allocate for the title.
    /// </summary>
    int _titleHeight = 14;

    /// <summary>
    /// Number of lines to allocate for a row of text..
    /// </summary>
    int _rowHeight = 10;

    /// <summary>
    /// The margin to leave on the left and right of the display.
    /// </summary>
    int _rowMargin = 0;

    /// <summary>
    /// Background colour for the display.
    /// </summary>
    Color _backgroundColor = Color.Black;

    /// <summary>
    /// Foreground colour for the text on the display.
    /// </summary>
    Color _foregroundColor = Color.White;

    /// <summary>
    /// Font to be used for the lines of text on the display.
    /// </summary>
    Font6x8 _textFont = new Font6x8();

    /// <summary>
    /// Font normally used for the title
    /// </summary>
    Font8x12 _titleFont = new Font8x12();

    /// <summary>
    /// Which line should be used for the next line of text.
    /// </summary>
    int _currentLine = 0;

    /// <summary>
    /// Display object to be used for 
    /// </summary>
    protected DisplayScreen DisplayScreen { get; private set; }

    /// <summary>
    /// Title text (normally the test being run).
    /// </summary>
    protected Label Title { get; private set; }

    /// <summary>
    /// Text on the display.
    /// </summary>
    protected Label[] Lines { get; private set; }

    /// <summary>
    /// Create a new instance of the DisplayController class.
    /// </summary>
    /// <param name="display">Display to be used.</param>
    public DisplayController(IGraphicsDisplay display)
    {
        if (display != null)
        {
            DisplayScreen = new DisplayScreen(display)
            {
                BackgroundColor = _backgroundColor
            };

            Title = new Label(_rowMargin, 0, DisplayScreen.Width - (2 * _rowMargin), _titleHeight)
            {
                Text = "",
                BackColor = _foregroundColor,
                TextColor = _backgroundColor,
                Font = _titleFont,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            DisplayScreen.Controls.Add(Title);

            Lines = new Label[2];
            for (int index = 0; index < Lines.Length; index++)
            {
                Lines[index] = new Label(_rowMargin, _titleHeight + (index * _rowHeight), DisplayScreen.Width - (2 * _rowMargin), _rowHeight)
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
        else
        {
            DisplayScreen = null;
        }
    }

    /// <summary>
    /// Clear the display.
    /// </summary>
    public void Clear()
    {
        if (DisplayScreen != null)
        {
            DisplayScreen.BeginUpdate();

            Title.Text = "";
            for (int index = 0; index < Lines.Length; index++)
            {
                Lines[index].Text = "";
            }

            DisplayScreen.EndUpdate();
        }
    }

    /// <summary>
    /// Add a line of text to the bottom of the displayed lines.  The display will
    /// be scrolled up one line if the bottom line is already full.
    /// </summary>
    /// <param name="text">Line of text to add to the displayed text.</param>
    private void AddText(string text)
    {
        DisplayScreen.BeginUpdate();

        if (_currentLine == 2)
        {
            Lines[0].Text = Lines[1].Text;
            Lines[1].Text = text;
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
        if (DisplayScreen != null)
        {
            DisplayScreen.BeginUpdate();

            Title.Text = title;

            DisplayScreen.EndUpdate();
        }
        else
        {
            ConsoleLog(title);
        }
    }

    /// <summary>
    /// Show the message on the display with a time stamp.
    /// </summary>
    /// <param name="message">Message to be shown.</param>
    /// <param addTimestamp="addTimestamp">Add a time stamp to the message?</param>
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
