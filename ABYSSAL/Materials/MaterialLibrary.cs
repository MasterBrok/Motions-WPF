using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ABYSSAL.Materials;

public class MaterialSet
{
    public DiffuseMaterial Diffuse { get; set; } = null!;
    public EmissiveMaterial? Emissive { get; set; }
    public SpecularMaterial? Specular { get; set; }
}

public class MaterialLibrary
{
    private static readonly Dictionary<string, MaterialSet> _materials = new();

    public static void Initialize()
    {
        _materials["weathered_stone"] = new MaterialSet
        {
            Diffuse = new DiffuseMaterial
            {
                Brush = new SolidColorBrush(Color.FromArgb(255, 35, 45, 60))
            }
        };

        _materials["dark_rock"] = new MaterialSet
        {
            Diffuse = new DiffuseMaterial
            {
                Brush = new SolidColorBrush(Color.FromArgb(255, 18, 25, 38))
            }
        };

        _materials["aged_bronze"] = new MaterialSet
        {
            Diffuse = new DiffuseMaterial
            {
                Brush = new LinearGradientBrush(
                    Color.FromArgb(255, 60, 50, 35),
                    Color.FromArgb(255, 40, 35, 25),
                    0)
            }
        };

        _materials["corroded_metal"] = new MaterialSet
        {
            Diffuse = new DiffuseMaterial
            {
                Brush = new SolidColorBrush(Color.FromArgb(255, 45, 50, 55))
            }
        };

        _materials["pale_mineral"] = new MaterialSet
        {
            Diffuse = new DiffuseMaterial
            {
                Brush = new SolidColorBrush(Color.FromArgb(255, 55, 65, 75))
            }
        };

        _materials["cyan_accent"] = new MaterialSet
        {
            Diffuse = new DiffuseMaterial
            {
                Brush = new SolidColorBrush(Color.FromArgb(255, 200, 170, 0))
            },
            Emissive = new EmissiveMaterial
            {
                Brush = new SolidColorBrush(Color.FromArgb(100, 255, 215, 0))
            }
        };

        _materials["emissive_rune"] = new MaterialSet
        {
            Diffuse = new DiffuseMaterial
            {
                Brush = new SolidColorBrush(Color.FromArgb(255, 40, 35, 20))
            },
            Emissive = new EmissiveMaterial
            {
                Brush = new SolidColorBrush(Color.FromArgb(180, 255, 200, 0))
            }
        };

        _materials["bioluminescent_coral"] = new MaterialSet
        {
            Diffuse = new DiffuseMaterial
            {
                Brush = new SolidColorBrush(Color.FromArgb(255, 80, 120, 40))
            },
            Emissive = new EmissiveMaterial
            {
                Brush = new RadialGradientBrush(
                    Color.FromArgb(200, 255, 215, 0),
                    Color.FromArgb(50, 100, 120, 0))
            }
        };

        _materials["glowing_crystal"] = new MaterialSet
        {
            Diffuse = new DiffuseMaterial
            {
                Brush = new SolidColorBrush(Color.FromArgb(255, 60, 70, 30))
            },
            Emissive = new EmissiveMaterial
            {
                Brush = new RadialGradientBrush(
                    Color.FromArgb(255, 255, 240, 100),
                    Color.FromArgb(100, 200, 180, 0))
            }
        };
    }

    public static MaterialSet? Get(string name)
    {
        return _materials.GetValueOrDefault(name);
    }

    public static DiffuseMaterial GetDiffuse(string name)
    {
        return _materials.GetValueOrDefault(name)?.Diffuse ?? new DiffuseMaterial();
    }

    public static EmissiveMaterial GetEmissive(string name)
    {
        return _materials.GetValueOrDefault(name)?.Emissive ?? new EmissiveMaterial();
    }
}
