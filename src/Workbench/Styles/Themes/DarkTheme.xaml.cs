using Avalonia.Markup.Xaml;
using AvaloniaStyles = Avalonia.Styling.Styles;

namespace Meadow.Workbench.Themes;

public class DarkTheme : AvaloniaStyles
{
    public DarkTheme() => AvaloniaXamlLoader.Load(this);
}
