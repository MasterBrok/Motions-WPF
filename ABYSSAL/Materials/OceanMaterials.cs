using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ABYSSAL.Materials;

public static class OceanMaterials
{
    public static DiffuseMaterial AbyssalNavy { get; } = new()
    {
        Brush = new SolidColorBrush(Color.FromArgb(255, 8, 15, 25))
    };

    public static DiffuseMaterial DeepOceanBlue { get; } = new()
    {
        Brush = new LinearGradientBrush(
            Color.FromArgb(255, 15, 30, 50),
            Color.FromArgb(255, 5, 15, 30),
            90)
    };

    public static DiffuseMaterial DesaturatedTeal { get; } = new()
    {
        Brush = new SolidColorBrush(Color.FromArgb(255, 45, 55, 35))
    };

    public static DiffuseMaterial BioluminescentCyan { get; } = new()
    {
        Brush = new SolidColorBrush(Color.FromArgb(255, 200, 170, 0))
    };

    public static DiffuseMaterial MutedTurquoise { get; } = new()
    {
        Brush = new SolidColorBrush(Color.FromArgb(255, 165, 160, 64))
    };

    public static DiffuseMaterial AgedBronze { get; } = new()
    {
        Brush = new LinearGradientBrush(
            Color.FromArgb(255, 60, 50, 35),
            Color.FromArgb(255, 40, 35, 25),
            0)
    };

    public static DiffuseMaterial PaleSilverBlue { get; } = new()
    {
        Brush = new SolidColorBrush(Color.FromArgb(255, 100, 130, 160))
    };

    public static EmissiveMaterial EmissiveCyan { get; } = new()
    {
        Brush = new SolidColorBrush(Color.FromArgb(150, 255, 215, 0))
    };

    public static EmissiveMaterial EmissiveTurquoise { get; } = new()
    {
        Brush = new RadialGradientBrush(
            Color.FromArgb(200, 100, 255, 50),
            Color.FromArgb(80, 120, 150, 0))
    };

    public static EmissiveMaterial EnergyGlow { get; } = new()
    {
        Brush = new RadialGradientBrush(
            Color.FromArgb(255, 255, 240, 100),
            Color.FromArgb(50, 180, 150, 0))
    };

    public static MaterialSet GetOceanDepthMaterial(double depth)
    {
        var t = Math.Clamp((depth + 80) / 120.0, 0, 1);

        var r = (byte)(15 - t * 10);
        var g = (byte)(25 - t * 15);
        var b = (byte)(40 - t * 20);

        return new MaterialSet
        {
            Diffuse = new DiffuseMaterial
            {
                Brush = new SolidColorBrush(Color.FromArgb(255, r, g, b))
            }
        };
    }
}
