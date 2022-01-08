using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Threading.Tasks;
using TestExplorer.ViewModels;

namespace TestExplorer.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public async void BrowseLocalAssemblies(object sender, RoutedEventArgs args)
        {
            var ofd = new OpenFolderDialog();
            var result = await ofd.ShowAsync(this);
            if (result != null)
            {
                (DataContext as MainWindowViewModel).LocalAssemblyPath = result;
            }
        }
    }
}
