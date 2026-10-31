using System.Windows.Media;
using System.Windows.Media.Media3D;
using ABYSSAL.Geometry;
using ABYSSAL.Materials;

namespace ABYSSAL.Scene;

public class TrenchScene
{
    private readonly TerrainFactory _terrain = new();

    public void Build(Model3DGroup root, double detail)
    {
        CreateCanyonWalls(root, detail);
        CreateCanyonFloor(root, detail);
        CreateRockFormations(root, detail);
        CreateUnderwaterCliffs(root, detail);
    }

    private void CreateCanyonWalls(Model3DGroup root, double detail)
    {
        var mesh = _terrain.GenerateCanyonMesh(200, 150, 300, (int)(30 * detail));

        var material = new DiffuseMaterial
        {
            Brush = new LinearGradientBrush(
                Color.FromArgb(255, 15, 25, 40),
                Color.FromArgb(255, 8, 15, 25),
                0)
        };

        var model = new GeometryModel3D
        {
            Geometry = mesh,
            Material = material,
            BackMaterial = material
        };

        root.Children.Add(model);
    }

    private void CreateCanyonFloor(Model3DGroup root, double detail)
    {
        var mesh = _terrain.GenerateTerrainMesh(-100, 30, 150, (int)(40 * detail));

        var material = new DiffuseMaterial
        {
            Brush = new LinearGradientBrush(
                Color.FromArgb(255, 20, 30, 45),
                Color.FromArgb(255, 10, 20, 35),
                90)
        };

        var model = new GeometryModel3D
        {
            Geometry = mesh,
            Material = material,
            BackMaterial = material
        };

        root.Children.Add(model);
    }

    private void CreateRockFormations(Model3DGroup root, double detail)
    {
        var random = new Random(42);
        var rockCount = (int)(15 * detail);

        for (int i = 0; i < rockCount; i++)
        {
            var x = random.NextDouble() * 160 - 80;
            var z = random.NextDouble() * 200 - 80;
            var y = -40 + random.NextDouble() * 20;

            var scale = 5 + random.NextDouble() * 15;
            var height = 10 + random.NextDouble() * 30;

            var mesh = _terrain.GenerateIrregularRock(scale, height, (int)(8 * detail));
            var transform = new TranslateTransform3D(x, y, z);

            var rotation = new AxisAngleRotation3D(
                new Vector3D(0, 1, 0),
                random.NextDouble() * 360);
            var rotateTransform = new RotateTransform3D(rotation);

            var group = new Transform3DGroup();
            group.Children.Add(transform);
            group.Children.Add(rotateTransform);

            var material = new DiffuseMaterial
            {
                Brush = new SolidColorBrush(Color.FromArgb(255, 25, 35, 50))
            };

            var model = new GeometryModel3D
            {
                Geometry = mesh,
                Material = material,
                BackMaterial = material,
                Transform = group
            };

            root.Children.Add(model);
        }
    }

    private void CreateUnderwaterCliffs(Model3DGroup root, double detail)
    {
        var random = new Random(123);
        var cliffCount = (int)(8 * detail);

        for (int i = 0; i < cliffCount; i++)
        {
            var x = random.NextDouble() * 200 - 100;
            var z = random.NextDouble() * 250 - 100;
            var baseY = -60;

            var mesh = _terrain.GenerateCliff(20 + random.NextDouble() * 30, 40 + random.NextDouble() * 40, (int)(10 * detail));
            var transform = new TranslateTransform3D(x, baseY, z);

            var material = new DiffuseMaterial
            {
                Brush = new SolidColorBrush(Color.FromArgb(255, 18, 28, 42))
            };

            var model = new GeometryModel3D
            {
                Geometry = mesh,
                Material = material,
                BackMaterial = material,
                Transform = transform
            };

            root.Children.Add(model);
        }
    }
}
