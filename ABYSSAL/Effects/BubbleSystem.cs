using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using ABYSSAL.Geometry;

namespace ABYSSAL.Effects;

public class BubbleSystem
{
    private readonly List<BubbleData> _bubbles = new();
    private readonly Random _seededRandom = new(2000);
    private Model3DGroup? _bubbleRoot;

    private class BubbleData
    {
        public Point3D Position;
        public double Radius;
        public double Speed;
        public double Wobble;
        public double Phase;
        public TranslateTransform3D Transform = null!;
        public EmissiveMaterial Material = null!;
    }

    public void Initialize(Model3DGroup root, QualityLevel quality)
    {
        _bubbleRoot = new Model3DGroup();
        root.Children.Add(_bubbleRoot);

        var count = quality switch
        {
            QualityLevel.Performance => 30,
            QualityLevel.Balanced => 60,
            QualityLevel.Cinematic => 100,
            _ => 60
        };

        var geometry = CreateGeometry.CreateSphere(0.5, 8, 6);

        for (int i = 0; i < count; i++)
        {
            var x = _seededRandom.NextDouble() * 250 - 50;
            var y = _seededRandom.NextDouble() * 150 - 80;
            var z = _seededRandom.NextDouble() * 250 - 50;

            var bubble = new BubbleData
            {
                Position = new Point3D(x, y, z),
                Radius = 0.1 + _seededRandom.NextDouble() * 0.4,
                Speed = 0.5 + _seededRandom.NextDouble() * 1.5,
                Wobble = 0.5 + _seededRandom.NextDouble(),
                Phase = _seededRandom.NextDouble() * Math.PI * 2
            };

            var transform = new TranslateTransform3D(x, y, z);

            var material = new EmissiveMaterial
            {
                Brush = new RadialGradientBrush(
                    Color.FromArgb(40, 245, 230, 200),
                    Color.FromArgb(10, 220, 200, 150))
            };
            ((RadialGradientBrush)material.Brush!).GradientOrigin = new Point(0.5, 0.5);
            ((RadialGradientBrush)material.Brush!).Center = new Point(0.5, 0.5);

            var scale = bubble.Radius;
            var scaledGeometry = ScaleGeometry(geometry, scale);

            var model = new GeometryModel3D
            {
                Geometry = scaledGeometry,
                Material = material,
                BackMaterial = material,
                Transform = transform
            };

            bubble.Transform = transform;
            bubble.Material = material;

            _bubbleRoot.Children.Add(model);
            _bubbles.Add(bubble);
        }
    }

    public void Update(TimeSpan time)
    {
        var seconds = time.TotalSeconds;

        foreach (var bubble in _bubbles)
        {
            var newY = bubble.Position.Y + seconds * bubble.Speed;

            if (newY > 85) newY = -70;

            var wobbleX = Math.Sin(seconds * bubble.Wobble + bubble.Phase) * 0.5;
            var wobbleZ = Math.Cos(seconds * bubble.Wobble * 0.7 + bubble.Phase) * 0.5;

            bubble.Transform.OffsetX = bubble.Position.X + wobbleX;
            bubble.Transform.OffsetY = newY;
            bubble.Transform.OffsetZ = bubble.Position.Z + wobbleZ;
        }
    }

    private MeshGeometry3D ScaleGeometry(MeshGeometry3D source, double scale)
    {
        var result = new MeshGeometry3D();

        foreach (var pos in source.Positions)
        {
            result.Positions.Add(new Point3D(pos.X * scale, pos.Y * scale, pos.Z * scale));
        }

        foreach (var idx in source.TriangleIndices)
        {
            result.TriangleIndices.Add(idx);
        }

        return result;
    }
}
