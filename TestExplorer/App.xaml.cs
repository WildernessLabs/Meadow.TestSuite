using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Meadow.Workbench;
using Meadow.Workbench.Themes;
using TestExplorer.Views;

namespace TestExplorer
{
    public class App : Application
    {
        public static Window MainWindow { get; set; } = null!;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);

            Styles.Add(new DarkTheme());
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };

                MainWindow = desktop.MainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
