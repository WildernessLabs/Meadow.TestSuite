using Meadow.Devices;
using Meadow.Foundation.Displays.TftSpi;
using Meadow.Foundation.Graphics;
using Meadow.Foundation;
using System;

namespace Meadow.TestSuite
{
    public class ST7789TestDisplay : ITestDisplay
    {
        private St7789 _display;
        private MicroGraphics _canvas;

        public ST7789TestDisplay(F7MicroBase f7)
        {
            _display = new St7789(f7, f7.CreateSpiBus(), f7.Pins.D15, f7.Pins.D11, f7.Pins.D14, 240, 240);
            _canvas = new MicroGraphics(_display);

            // TODO: this should probably be configureable
            _canvas.Rotation = RotationType._180Degrees;
            _canvas.Clear(true);

            _canvas.CurrentFont = new Font12x20();

        }

        public void ShowText(int line, string text)
        {
            var lineheight = 20;

            var y = 5 + (line * lineheight);
            _canvas.DrawRectangle(0, y, _canvas.Width, 20, Color.Black, true);
            _canvas.DrawText(5, y, text, Color.White);
            _canvas.Show();
        }
    }
}