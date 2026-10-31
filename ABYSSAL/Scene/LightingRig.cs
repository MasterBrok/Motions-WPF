using System.Windows.Media;
using System.Windows.Media.Media3D;
using ABYSSAL.Geometry;
using ABYSSAL.Materials;

namespace ABYSSAL.Scene;

public class LightingRig
{
    private readonly List<Light> _lights = new();
    private DirectionalLight? _ambientLight;
    private DirectionalLight? _primaryLight;
    private PointLight? _sanctuaryLight;
    private PointLight? _biolumLight1;
    private PointLight? _biolumLight2;

    public IEnumerable<Light> GetAllLights() => _lights;

    public LightingRig()
    {
        Initialize();
    }

    private void Initialize()
    {
        _ambientLight = new DirectionalLight
        {
            Color = Color.FromArgb(255, 100, 130, 90),
            Direction = new Vector3D(0.2, -1, 0.3)
        };
        _lights.Add(_ambientLight);

        _primaryLight = new DirectionalLight
        {
            Color = Color.FromArgb(255, 140, 160, 110),
            Direction = new Vector3D(0.3, -1, 0.5)
        };
        _lights.Add(_primaryLight);

        _sanctuaryLight = new PointLight
        {
            Color = Color.FromArgb(255, 255, 215, 0),
            Position = new Point3D(100, -65, 100),
            Range = 100,
            ConstantAttenuation = 0.005f,
            LinearAttenuation = 0.002f,
            QuadraticAttenuation = 0.0005f
        };
        _lights.Add(_sanctuaryLight);

        _biolumLight1 = new PointLight
        {
            Color = Color.FromArgb(255, 100, 255, 50),
            Position = new Point3D(85, -52, 85),
            Range = 40,
            ConstantAttenuation = 0.01f
        };
        _lights.Add(_biolumLight1);

        _biolumLight2 = new PointLight
        {
            Color = Color.FromArgb(255, 255, 215, 0),
            Position = new Point3D(50, -45, 50),
            Range = 35,
            ConstantAttenuation = 0.02f
        };
        _lights.Add(_biolumLight2);
    }

    public void Update(TimeSpan time)
    {
        var seconds = time.TotalSeconds;

        var pulseIntensity = (Math.Sin(seconds * 0.5) + 1) / 2;
        if (_sanctuaryLight != null)
        {
            _sanctuaryLight.Color = Color.FromArgb(255,
                (byte)(255),
                (byte)(215 - pulseIntensity * 30),
                (byte)(pulseIntensity * 30));
        }

        var surfaceLight = (Math.Sin(seconds * 0.3) * 0.2 + 0.8);
        if (_primaryLight != null)
        {
            _primaryLight.Color = Color.FromArgb(255,
                (byte)(100 * surfaceLight),
                (byte)(130 * surfaceLight),
                (byte)(90 * surfaceLight));
        }
    }

    public void SetSanctuaryGlow(bool active)
    {
        if (_sanctuaryLight != null)
        {
            _sanctuaryLight.Color = active
                ? Color.FromArgb(255, 255, 240, 100)
                : Color.FromArgb(255, 255, 215, 0);
        }
    }
}
