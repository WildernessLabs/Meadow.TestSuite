using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Meadow.Workbench.Views
{
    public partial class CliView : UserControl
    {
        public CliView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
