using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using ABYSSAL.Geometry;

namespace ABYSSAL.Effects;

public class BioluminescenceSystem
{
    private readonly List<GlowPoint> _glowPoints = new();
    private readonly Random _seededRandom = new(3000);
    private Model3DGroup? _glowRoot;
    private TimelineController? _timeline;

    private class GlowPoint
    {
        public Point3D Position;
        public double Intensity;
        public double Phase;
        public double PulseSpeed;
        public EmissiveMaterial Material = null!;
        public TranslateTransform3D Transform = null!;
    }

    public void Initialize(Model3DGroup root, QualityLevel quality)
    {
        _glowRoot = new Model3DGroup();
        root.Children.Add(_glowRoot);

        var count = quality switch
        {
            QualityLevel.Performance => 40,
            QualityLevel.Balanced => 80,
            QualityLevel.Cinematic => 150,
            _ => 80
        };

        var glowGeometry = CreateGeometry.CreateSphere(0.3, 6, 4);

        for (int i = 0; i < count; i++)
        {
            var x = _seededRandom.NextDouble() * 180 + 10;
            var y = _seededRandom.NextDouble() * 30 - 65;
            var z = _seededRandom.NextDouble() * 180 + 10;

            var glow = new GlowPoint
            {
                Position = new Point3D(x, y, z),
                Intensity = 0.3 + _seededRandom.NextDouble() * 0.7,
                Phase = _seededRandom.NextDouble() * Math.PI * 2,
                PulseSpeed = 0.5 + _seededRandom.NextDouble() * 1.5
            };

            var transform = new TranslateTransform3D(x, y, z);

            var baseColor = _seededRandom.NextDouble() > 0.5
                ? Color.FromArgb(200, 255, 215, 0)
                : Color.FromArgb(180, 100, 255, 50);

            var material = new EmissiveMaterial
            {
                Brush = new RadialGradientBrush(
                    baseColor,
                    Color.FromArgb(50, 0, 100, 120))
            };
            ((RadialGradientBrush)material.Brush!).GradientOrigin = new Point(0.5, 0.5);
            ((RadialGradientBrush)material.Brush!).Center = new Point(0.5, 0.5);

            var model = new GeometryModel3D
            {
                Geometry = glowGeometry,
                Material = material,
                BackMaterial = material,
                Transform = transform
            };

            glow.Transform = transform;
            glow.Material = material;

            _glowRoot!.Children.Add(model);
            _glowPoints.Add(glow);
        }
    }

    public void InitializeAnimations(TimelineController timeline)
    {
        _timeline = timeline;
    }

    public void Update(TimeSpan time, QualityLevel quality)
    {
        if (_timeline == null) return;

        var seconds = time.TotalSeconds;

        foreach (var glow in _glowPoints)
        {
            var pulse = (Math.Sin(seconds * glow.PulseSpeed + glow.Phase) + 1) / 2;
            var intensity = glow.Intensity * (0.5 + pulse * 0.5);

            if (time.TotalSeconds >= 200)
            {
                intensity *= 1.5;
            }

            var baseAlpha = (byte)(intensity * 200);
            var color = Color.FromArgb(baseAlpha, 255, 215, 0);

            glow.Material.Brush = new RadialGradientBrush(
                color,
                Color.FromArgb((byte)(intensity * 100), 100, 150, 0));
            ((RadialGradientBrush)glow.Material.Brush).GradientOrigin = new Point(0.5, 0.5);
            ((RadialGradientBrush)glow.Material.Brush).Center = new Point(0.5, 0.5);
        }
    }
}
