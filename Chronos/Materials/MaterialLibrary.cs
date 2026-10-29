using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Chronos.Materials;

public static class MaterialLibrary
{
    private static readonly SolidColorBrush BlackTitaniumBrush;
    private static readonly SolidColorBrush BrushedGoldBrush;
    private static readonly SolidColorBrush PolishedChromeBrush;
    private static readonly SolidColorBrush CyanEmissiveBrush;

    private static readonly DiffuseMaterial BlackTitaniumDiffuse;
    private static readonly SpecularMaterial BlackTitaniumSpecular;
    private static readonly DiffuseMaterial BrushedGoldDiffuse;
    private static readonly SpecularMaterial BrushedGoldSpecular;
    private static readonly DiffuseMaterial PolishedChromeDiffuse;
    private static readonly SpecularMaterial PolishedChromeSpecular;
    private static readonly EmissiveMaterial CyanEmissive;

    static MaterialLibrary()
    {
        BlackTitaniumBrush = new SolidColorBrush(Color.FromRgb(70, 75, 85));
        BlackTitaniumBrush.Freeze();

        BrushedGoldBrush = new SolidColorBrush(Color.FromRgb(220, 180, 100));
        BrushedGoldBrush.Freeze();

        PolishedChromeBrush = new SolidColorBrush(Color.FromRgb(210, 215, 225));
        PolishedChromeBrush.Freeze();

        CyanEmissiveBrush = new SolidColorBrush(Color.FromRgb(0, 220, 255));
        CyanEmissiveBrush.Freeze();

        BlackTitaniumDiffuse = new DiffuseMaterial(BlackTitaniumBrush) { AmbientColor = Color.FromRgb(60, 60, 70) };
        BlackTitaniumSpecular = new SpecularMaterial(new SolidColorBrush(Color.FromRgb(200, 200, 210)), 90);

        BrushedGoldDiffuse = new DiffuseMaterial(BrushedGoldBrush) { AmbientColor = Color.FromRgb(120, 95, 45) };
        BrushedGoldSpecular = new SpecularMaterial(new SolidColorBrush(Color.FromRgb(255, 230, 150)), 80);

        PolishedChromeDiffuse = new DiffuseMaterial(PolishedChromeBrush) { AmbientColor = Color.FromRgb(140, 145, 155) };
        PolishedChromeSpecular = new SpecularMaterial(new SolidColorBrush(Color.FromRgb(255, 255, 255)), 120);

        CyanEmissive = new EmissiveMaterial(CyanEmissiveBrush);
    }

    public static MaterialGroup CreateBlackTitanium()
    {
        var group = new MaterialGroup();
        group.Children.Add(BlackTitaniumDiffuse);
        group.Children.Add(BlackTitaniumSpecular);
        group.Freeze();
        return group;
    }

    public static MaterialGroup CreateBrushedGold()
    {
        var group = new MaterialGroup();
        group.Children.Add(BrushedGoldDiffuse);
        group.Children.Add(BrushedGoldSpecular);
        group.Freeze();
        return group;
    }

    public static MaterialGroup CreatePolishedChrome()
    {
        var group = new MaterialGroup();
        group.Children.Add(PolishedChromeDiffuse);
        group.Children.Add(PolishedChromeSpecular);
        group.Freeze();
        return group;
    }

    public static EmissiveMaterial CreateCyanEmissive(double intensity = 1.0)
    {
        var brush = new SolidColorBrush(Color.FromRgb(
            (byte)(0 * intensity),
            (byte)(220 * intensity),
            (byte)(255 * intensity)));
        brush.Freeze();

        var material = new EmissiveMaterial(brush);
        return material;
    }

    public static MaterialGroup CreateCyanCore()
    {
        var group = new MaterialGroup();
        group.Children.Add(CreateCyanEmissive(0.6));
        group.Children.Add(new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(0, 100, 120))) { AmbientColor = Color.FromRgb(0, 40, 50) });
        group.Freeze();
        return group;
    }
}