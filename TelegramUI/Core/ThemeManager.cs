using System.Windows;
using System.Windows.Media.Animation;

namespace TelegramUI.Core;

public static class ThemeManager
{
    private static readonly ResourceDictionary DarkTheme = new()
    {
        Source = new Uri("/Themes/DarkTheme.xaml", UriKind.Relative)
    };

    private static readonly ResourceDictionary LightTheme = new()
    {
        Source = new Uri("/Themes/LightTheme.xaml", UriKind.Relative)
    };

    public static bool IsDark { get; private set; } = true;

    public static event EventHandler<bool>? ThemeChanged;

    public static void Apply(bool dark)
    {
        var app = Application.Current;
        if (app == null) return;

        var md = app.Resources.MergedDictionaries;
        var existing = md.FirstOrDefault(d =>
            d.Source?.OriginalString.Contains("Theme.xaml", StringComparison.OrdinalIgnoreCase) == true);
        if (existing != null) md.Remove(existing);

        md.Insert(0, dark ? DarkTheme : LightTheme);
        IsDark = dark;
        ThemeChanged?.Invoke(null, dark);
    }

    public static void Toggle()
    {
        Apply(!IsDark);
    }
}
