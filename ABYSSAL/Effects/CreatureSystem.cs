using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using ABYSSAL.Geometry;

namespace ABYSSAL.Effects;

public class CreatureSystem
{
    private readonly List<CreatureData> _creatures = new();
    private readonly Random _seededRandom = new(4000);
    private Model3DGroup? _creatureRoot;
    private TimelineController? _timeline;

    private class CreatureData
    {
        public CreatureType Type;
        public Point3D Position;
        public Point3D StartPosition;
        public Vector3D Velocity;
        public double Size;
        public double Phase;
        public double Speed;
        public double WingPhase;
        public TranslateTransform3D Transform = null!;
        public RotateTransform3D Rotation = null!;
    }

    private enum CreatureType
    {
        Fish,
        Jellyfish,
        LargeCreature
    }

    public void Initialize(Model3DGroup root, int count, QualityLevel quality)
    {
        _creatureRoot = new Model3DGroup();
        root.Children.Add(_creatureRoot);

        var fishCount = count * 6 / 10;
        var jellyCount = count * 3 / 10;
        var largeCount = count * 1 / 10;

        CreateFish(fishCount);
        CreateJellyfish(jellyCount);
        CreateLargeCreatures(largeCount);
    }

    private void CreateFish(int count)
    {
        var bodyGeometry = CreateFishBody();
        var finGeometry = CreateFishFin();

        for (int i = 0; i < count; i++)
        {
            var x = _seededRandom.NextDouble() * 250 - 50;
            var y = _seededRandom.NextDouble() * 100 - 60;
            var z = _seededRandom.NextDouble() * 250 - 50;

            var speed = 2 + _seededRandom.NextDouble() * 3;
            var direction = new Vector3D(
                (_seededRandom.NextDouble() - 0.5) * 2,
                (_seededRandom.NextDouble() - 0.5) * 0.3,
                (_seededRandom.NextDouble() - 0.5) * 2
            );
            direction.Normalize();

            var creature = new CreatureData
            {
                Type = CreatureType.Fish,
                Position = new Point3D(x, y, z),
                StartPosition = new Point3D(x, y, z),
                Velocity = direction,
                Size = 0.3 + _seededRandom.NextDouble() * 0.5,
                Phase = _seededRandom.NextDouble() * Math.PI * 2,
                Speed = speed
            };

            var transform = new TranslateTransform3D(x, y, z);

            var lookDir = creature.Velocity;
            var angle = Math.Atan2(lookDir.X, lookDir.Z) * 180 / Math.PI;
            var rotation = new RotateTransform3D(
                new AxisAngleRotation3D(new Vector3D(0, 1, 0), angle));

            var group = new Transform3DGroup();
            group.Children.Add(transform);
            group.Children.Add(rotation);

            var material = new DiffuseMaterial
            {
                Brush = new SolidColorBrush(Color.FromArgb(200, 100, 150, 180))
            };

            var model = new GeometryModel3D
            {
                Geometry = bodyGeometry,
                Material = material,
                Transform = group
            };

            creature.Transform = transform;
            creature.Rotation = rotation;

            _creatureRoot!.Children.Add(model);
            _creatures.Add(creature);
        }
    }

    private MeshGeometry3D CreateFishBody()
    {
        return CreateGeometry.CreateSphere(1, 8, 6);
    }

    private MeshGeometry3D CreateFishFin()
    {
        return CreateGeometry.CreateBox(0.3, 1, 0.1);
    }

    private void CreateJellyfish(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var x = _seededRandom.NextDouble() * 150 + 30;
            var y = _seededRandom.NextDouble() * 40 - 75;
            var z = _seededRandom.NextDouble() * 150 + 30;

            var creature = new CreatureData
            {
                Type = CreatureType.Jellyfish,
                Position = new Point3D(x, y, z),
                StartPosition = new Point3D(x, y, z),
                Size = 0.5 + _seededRandom.NextDouble() * 1,
                Phase = _seededRandom.NextDouble() * Math.PI * 2,
                Speed = 0.1 + _seededRandom.NextDouble() * 0.3,
                WingPhase = _seededRandom.NextDouble() * Math.PI * 2
            };

            var transform = new TranslateTransform3D(x, y, z);

            var bellGeometry = CreateGeometry.CreateSphere(creature.Size, 8, 6);
            var material = new EmissiveMaterial
            {
                Brush = new RadialGradientBrush(
                    Color.FromArgb(150, 255, 215, 0),
                    Color.FromArgb(50, 150, 120, 0))
            };
            ((RadialGradientBrush)material.Brush!).GradientOrigin = new Point(0.5, 0.5);
            ((RadialGradientBrush)material.Brush!).Center = new Point(0.5, 0.5);

            var model = new GeometryModel3D
            {
                Geometry = bellGeometry,
                Material = material,
                BackMaterial = material,
                Transform = transform
            };

            creature.Transform = transform;

            _creatureRoot!.Children.Add(model);
            _creatures.Add(creature);
        }
    }

    private void CreateLargeCreatures(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var x = _seededRandom.NextDouble() * 300 - 50;
            var y = _seededRandom.NextDouble() * 50 - 40;
            var z = _seededRandom.NextDouble() * 300 - 50;

            var creature = new CreatureData
            {
                Type = CreatureType.LargeCreature,
                Position = new Point3D(x, y, z),
                StartPosition = new Point3D(x, y, z),
                Size = 3 + _seededRandom.NextDouble() * 5,
                Phase = _seededRandom.NextDouble() * Math.PI * 2,
                Speed = 1 + _seededRandom.NextDouble() * 2
            };

            var transform = new TranslateTransform3D(x, y, z);

            var geometry = CreateGeometry.CreateSphere(creature.Size, 8, 6);
            var material = new DiffuseMaterial
            {
                Brush = new SolidColorBrush(Color.FromArgb(50, 30, 50, 70))
            };

            var model = new GeometryModel3D
            {
                Geometry = geometry,
                Material = material,
                Transform = transform
            };

            creature.Transform = transform;

            _creatureRoot!.Children.Add(model);
            _creatures.Add(creature);
        }
    }

    public void InitializeAnimations(TimelineController timeline)
    {
        _timeline = timeline;
    }

    public void Update(TimeSpan time)
    {
        if (_timeline == null) return;

        var seconds = time.TotalSeconds;

        foreach (var creature in _creatures)
        {
            switch (creature.Type)
            {
                case CreatureType.Fish:
                    UpdateFish(creature, seconds);
                    break;
                case CreatureType.Jellyfish:
                    UpdateJellyfish(creature, seconds);
                    break;
                case CreatureType.LargeCreature:
                    UpdateLargeCreature(creature, seconds);
                    break;
            }
        }
    }

    private void UpdateFish(CreatureData fish, double seconds)
    {
        var movement = fish.Speed * 0.5;
        var sway = Math.Sin(seconds * 2 + fish.Phase) * 0.5;

        var newX = fish.StartPosition.X + fish.Velocity.X * seconds * movement + sway;
        var newY = fish.StartPosition.Y + Math.Sin(seconds * 0.5 + fish.Phase) * 2;
        var newZ = fish.StartPosition.Z + fish.Velocity.Z * seconds * movement;

        if (newX < -100) newX += 350;
        if (newX > 250) newX -= 350;
        if (newZ < -100) newZ += 350;
        if (newZ > 250) newZ -= 350;

        fish.Transform.OffsetX = newX;
        fish.Transform.OffsetY = newY;
        fish.Transform.OffsetZ = newZ;

        var angle = Math.Atan2(fish.Velocity.X, fish.Velocity.Z) * 180 / Math.PI;
        fish.Rotation.Rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), angle + Math.Sin(seconds * 4) * 5);
    }

    private void UpdateJellyfish(CreatureData jelly, double seconds)
    {
        var pulse = (Math.Sin(seconds * 1.5 + jelly.Phase) + 1) / 2;

        var newY = jelly.StartPosition.Y + Math.Sin(seconds * jelly.Speed + jelly.Phase) * 3;
        var sway = Math.Sin(seconds * 0.3 + jelly.Phase) * 2;

        jelly.Transform.OffsetX = jelly.StartPosition.X + sway;
        jelly.Transform.OffsetY = newY;
        jelly.Transform.OffsetZ = jelly.StartPosition.Z + Math.Cos(seconds * 0.2 + jelly.Phase) * 1.5;
    }

    private void UpdateLargeCreature(CreatureData creature, double seconds)
    {
        var slowMove = creature.Speed * 0.2;

        creature.Transform.OffsetX = creature.StartPosition.X + Math.Sin(seconds * slowMove + creature.Phase) * 20;
        creature.Transform.OffsetZ = creature.StartPosition.Z + Math.Cos(seconds * slowMove * 0.7 + creature.Phase) * 20;
        creature.Transform.OffsetY = creature.StartPosition.Y + Math.Sin(seconds * 0.1 + creature.Phase) * 5;
    }
}
