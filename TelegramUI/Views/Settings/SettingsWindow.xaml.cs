using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TelegramUI.Core;

namespace TelegramUI.Views.Settings;

public partial class SettingsWindow : Window
{
    public event EventHandler<bool>? ReducedMotionChanged;
    public event EventHandler<bool>? MockTimerChanged;

    public SettingsWindow(bool isDark, bool reducedMotion, bool mockTimerEnabled)
    {
        InitializeComponent();
        ThemeToggle.IsChecked = isDark;
        MotionToggle.IsChecked = reducedMotion;
        MockTimerToggle.IsChecked = mockTimerEnabled;
    }

    private void ThemeToggle_Changed(object sender, RoutedEventArgs e)
    {
        if (ThemeToggle.IsChecked.HasValue)
            ThemeManager.Apply(ThemeToggle.IsChecked.Value);
    }

    private void MotionToggle_Changed(object sender, RoutedEventArgs e)
    {
        ReducedMotionChanged?.Invoke(this, MotionToggle.IsChecked == true);
    }

    private void MockTimer_Changed(object sender, RoutedEventArgs e)
    {
        MockTimerChanged?.Invoke(this, MockTimerToggle.IsChecked == true);
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
            DragMove();
    }
}
