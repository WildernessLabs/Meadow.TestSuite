using Meadow;
using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;
using Meadow.Peripherals.Displays;
using System;

namespace DisplayControllers;

public interface IDisplayController
{
    /// <summary>
    /// Clear the entire display including the header and footer.
    /// </summary>
    public void Clear();

    /// <summary>
    /// Clear the lines of text from the display.  The title and footer will be left alone.
    /// </summary>
    public void ClearText();

    /// <summary>
    /// Add a line of text to the bottom of the displayed lines.  The display will
    /// be scrolled up one line if the bottom line is already full.
    /// </summary>
    /// <param name="text">Line of text to add to the displayed text.</param>
    public void AddText(string text);

    /// <summary>
    /// Update the title line on the display.
    /// </summary>
    /// <param name="title">Title text.</param>
    public void UpdateTitle(string title);

    /// <summary>
    /// Update the footer line on the display.
    /// </summary>
    /// <param name="footer">Footer text.</param>
    public void UpdateFooter(string footer);

    /// <summary>
    /// Update the display with the specified lines.
    /// </summary>
    /// <param name="lines">Lines of text to display.</param>
    public void UpdateText(string[] lines);
    
    /// <summary>
    /// Show the message passed in on the display with a time stamp.
    /// </summary>
    /// <param name="message">Message to be shown.</param>
    public void Log(string message, bool addTimestamp = true);
    
    /// <summary>
    /// Send a message to the console.
    /// </summary>
    /// <remarks>
    /// This method is provided for when the display is not available.
    /// </remarks>
    /// <param name="message">Message to display.</param>
    public void ConsoleLog(string message);
}
