using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Motion3.Controls;

/// <summary>
/// Holographic geometric centerpiece: hex wireframe core, counter-rotating orbital
/// rings, radial ticks. XAML x:Name elements are exposed as generated fields; the
/// central timeline in MainWindow targets them directly (no name-scope lookups).
/// </summary>
public partial class GeometricCore : UserControl
{
    public GeometricCore()
    {
        InitializeComponent();
        BuildTicks();
    }

    private void BuildTicks()
    {
        var brush = (SolidColorBrush)(FindResource("BrushCyanDim") ?? new SolidColorBrush(Color.FromRgb(0, 184, 204)));
        var copy = new SolidColorBrush(brush.Color) { Opacity = 0.55 };
        copy.Freeze();

        for (int i = 0; i < 8; i++)
        {
            double angle = i * Math.PI / 4.0;
            double r1 = 46;
            double r2 = 52 + (i % 2) * 10;

            TicksCanvas.Children.Add(new Line
            {
                X1 = Math.Cos(angle) * r1,
                Y1 = Math.Sin(angle) * r1,
                X2 = Math.Cos(angle) * r2,
                Y2 = Math.Sin(angle) * r2,
                Stroke = copy,
                StrokeThickness = 0.7,
            });
        }
    }
}
