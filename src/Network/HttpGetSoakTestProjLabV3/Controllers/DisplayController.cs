using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;

using System;

namespace ProjectLabTest.Services
{
    internal class DisplayController
    {
        int _titleHeight = 44;

        int _rowHeight = 14;

        int _rowMargin = 5;

        Color _backgroundColor = Color.Black;

        Color _foregroundColor = Color.White;

        Font12x20 _titleFont = new Font12x20();

        Font8x12 _textFont = new Font8x12();

        int _currentLine = 0;

        protected DisplayScreen DisplayScreen { get; set; }

        protected Label Title { get; set; }

        protected Label[] Lines { get; set; }

        public DisplayController(IGraphicsDisplay display)
        {
            DisplayScreen = new DisplayScreen(display, RotationType._270Degrees)
            {
                BackgroundColor = _backgroundColor
            };

            Title = new Label(_rowMargin, 0, DisplayScreen.Width - (2 * _rowMargin), _titleHeight)
            {
                Text = "",
                TextColor = _foregroundColor,
                Font = _titleFont,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            DisplayScreen.Controls.Add(Title);

            Lines = new Label[14];
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

            if (_currentLine == Lines.Length - 1)
            {
                for (int index = 1; index < Lines.Length - 1; index++)
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
        public void Log(string message)
        {
            AddText($"{DateTime.Now.ToString("HH:mm:ss")}: {message}");
        }
    }
}