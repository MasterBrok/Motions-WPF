using System.Windows;
using TelegramUI.Core;

namespace TelegramUI;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ThemeManager.Apply(true);
    }
}
