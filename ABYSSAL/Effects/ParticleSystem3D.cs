using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using ABYSSAL.Geometry;

namespace ABYSSAL.Effects;

public class ParticleSystem3D
{
    private readonly List<ParticleData> _particles = new();
    private readonly Random _seededRandom = new(1000);
    private Model3DGroup? _particleRoot;
    private int _maxParticles;

    private class ParticleData
    {
        public Point3D Position;
        public Vector3D Velocity;
        public double Size;
        public double Phase;
        public double Speed;
        public Model3D? Model;
    }

    public void Initialize(Model3DGroup root, int count)
    {
        _maxParticles = count;
        _particleRoot = new Model3DGroup();
        root.Children.Add(_particleRoot);

        var geometry = CreateGeometry.CreateSphere(0.1, 6, 4);

        var material = new DiffuseMaterial
        {
            Brush = new System.Windows.Media.SolidColorBrush(
                System.Windows.Media.Color.FromArgb(120, 220, 220, 180))
        };

        for (int i = 0; i < count; i++)
        {
            var particle = new ParticleData
            {
                Position = new Point3D(
                    _seededRandom.NextDouble() * 400 - 200,
                    _seededRandom.NextDouble() * 150 - 80,
                    _seededRandom.NextDouble() * 400 - 200),
                Velocity = new Vector3D(
                    (_seededRandom.NextDouble() - 0.5) * 0.5,
                    -0.3 - _seededRandom.NextDouble() * 0.5,
                    (_seededRandom.NextDouble() - 0.5) * 0.5),
                Size = 0.05 + _seededRandom.NextDouble() * 0.15,
                Phase = _seededRandom.NextDouble() * Math.PI * 2,
                Speed = 0.3 + _seededRandom.NextDouble() * 0.7
            };

            var transform = new TranslateTransform3D(particle.Position.X, particle.Position.Y, particle.Position.Z);

            var model = new GeometryModel3D
            {
                Geometry = geometry,
                Material = material,
                Transform = transform
            };

            particle.Model = model;
            _particleRoot.Children.Add(model);
            _particles.Add(particle);
        }
    }

    public void Update(TimeSpan time)
    {
        var seconds = time.TotalSeconds;

        for (int i = 0; i < _particles.Count; i++)
        {
            var p = _particles[i];
            if (p.Model == null) continue;

            var sway = Math.Sin(seconds * p.Speed + p.Phase) * 2.0;

            var x = p.Position.X + p.Velocity.X * seconds + sway;
            var y = p.Position.Y + p.Velocity.Y * seconds * 10;
            var z = p.Position.Z + p.Velocity.Z * seconds + Math.Cos(seconds * p.Speed * 0.7 + p.Phase) * 1.5;

            if (y < -80) y = 80;

            if (p.Model.Transform is TranslateTransform3D t)
            {
                t.OffsetX = x;
                t.OffsetY = y;
                t.OffsetZ = z;
            }
        }
    }
}
