using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Meadow.Workbench.Views
{
    public partial class TestSuiteView : UserControl
    {
        public TestSuiteView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public async void BrowseLocalAssemblies(object sender, RoutedEventArgs args)
        {
            var ofd = new OpenFolderDialog();
            var w = this.Parent as Window;

            var result = await ofd.ShowAsync(TestExplorer.App.MainWindow);
            if (result != null)
            {
                (DataContext as TestSuiteViewModel).LocalAssemblyPath = result;
            }
        }

    }
}
