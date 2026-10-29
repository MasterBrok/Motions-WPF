using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Chronos.Geometry;
using Chronos.Materials;
using Chronos.Models;

namespace Chronos.Scene;

public class SceneBuilder
{
    private readonly Model3DGroup _rootModel = new();
    private readonly LightingRig _lightingRig = new();
    private readonly MechanicalHeart _heart = new();
    private Point3D _heartCenter = new(0, 0, 0);

    public Point3D HeartCenter => _heartCenter;
    public LightingRig Lighting => _lightingRig;
    public MechanicalHeart Heart => _heart;

    public void BuildScene(Model3DGroup parentGroup)
    {
        _heart.CreateHeart(_rootModel);
        foreach (var child in _rootModel.Children)
            parentGroup.Children.Add(child);
    }

    public Point3D GetHeartCenter() => _heartCenter;
}

public class LightingRig
{
    public AmbientLight Ambient { get; }
    public DirectionalLight KeyLight { get; }
    public DirectionalLight FillLight { get; }
    public DirectionalLight RimLight { get; }
    public PointLight CoreLight { get; }

    public LightingRig()
    {
        Ambient = new AmbientLight(Color.FromArgb(255, 110, 115, 125));

        KeyLight = new DirectionalLight
        {
            Color = Colors.White,
            Direction = new Vector3D(0.3, -0.6, -0.9)
        };

        FillLight = new DirectionalLight
        {
            Color = Color.FromArgb(255, 90, 110, 130),
            Direction = new Vector3D(-0.6, 0.2, -0.6)
        };

        RimLight = new DirectionalLight
        {
            Color = Color.FromArgb(255, 0, 160, 190),
            Direction = new Vector3D(0, 0.2, 1)
        };

        CoreLight = new PointLight
        {
            Color = Color.FromArgb(255, 0, 200, 230),
            Position = new Point3D(0, 0, 0),
            Range = 12,
            ConstantAttenuation = 0.03f
        };
    }

    public List<Light> GetAllLights() => new() { Ambient, KeyLight, FillLight, RimLight, CoreLight };
}