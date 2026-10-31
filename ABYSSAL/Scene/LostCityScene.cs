using System.Windows.Media;
using System.Windows.Media.Media3D;
using ABYSSAL.Geometry;
using ABYSSAL.Materials;

namespace ABYSSAL.Scene;

public class LostCityScene
{
    private readonly ArchitectureFactory _arch = new();
    private readonly MonumentFactory _monuments = new();
    private readonly CoralFactory _coral = new();
    private readonly TerrainFactory _terrain = new();

    public void Build(Model3DGroup root, double detail)
    {
        CreateCityBase(root, detail);
        CreateTempleComplex(root, detail);
        CreateMonumentalStatues(root, detail);
        CreateAvenueOfColumns(root, detail);
        CreateAncientBridges(root, detail);
        CreateArchways(root, detail);
        CreateBioLuminescentGarden(root, detail);
        CreateBrokenObservatory(root, detail);
        CreateCityDetails(root, detail);
    }

    private void CreateCityBase(Model3DGroup root, double detail)
    {
        var mesh = new MeshGeometry3D();
        var size = 250.0;
        var segments = (int)(60 * detail);

        for (int z = 0; z <= segments; z++)
        {
            for (int x = 0; x <= segments; x++)
            {
                var px = (x / (double)segments - 0.5) * size;
                var pz = (z / (double)segments - 0.5) * size;
                var py = -70;

                mesh.Positions.Add(new Point3D(px, py, pz));
            }
        }

        for (int z = 0; z < segments; z++)
        {
            for (int x = 0; x < segments; x++)
            {
                var idx = z * (segments + 1) + x;
                mesh.TriangleIndices.Add(idx);
                mesh.TriangleIndices.Add(idx + segments + 1);
                mesh.TriangleIndices.Add(idx + 1);

                mesh.TriangleIndices.Add(idx + 1);
                mesh.TriangleIndices.Add(idx + segments + 1);
                mesh.TriangleIndices.Add(idx + segments + 2);
            }
        }

        var material = new DiffuseMaterial
        {
            Brush = new LinearGradientBrush(
                Color.FromArgb(255, 25, 35, 50),
                Color.FromArgb(255, 15, 25, 40),
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

    private void CreateTempleComplex(Model3DGroup root, double detail)
    {
        var templeX = 70;
        var templeZ = 70;

        var platform = _arch.GeneratePlatform(40, 5, 30);
        var platformTransform = new TranslateTransform3D(templeX, -68, templeZ);
        var platformMaterial = new DiffuseMaterial
        {
            Brush = new SolidColorBrush(Color.FromArgb(255, 40, 50, 65))
        };
        var platformModel = new GeometryModel3D
        {
            Geometry = platform,
            Material = platformMaterial,
            BackMaterial = platformMaterial,
            Transform = platformTransform
        };
        root.Children.Add(platformModel);

        var pyramid = _arch.GeneratePyramid(25, 20, 4);
        var pyramidTransform = new TranslateTransform3D(templeX, -48, templeZ);
        var pyramidMaterial = new DiffuseMaterial
        {
            Brush = new LinearGradientBrush(
                Color.FromArgb(255, 35, 45, 60),
                Color.FromArgb(255, 25, 35, 50),
                45)
        };
        var pyramidModel = new GeometryModel3D
        {
            Geometry = pyramid,
            Material = pyramidMaterial,
            BackMaterial = pyramidMaterial,
            Transform = pyramidTransform
        };
        root.Children.Add(pyramidModel);

        var stepCount = (int)(5 * detail);
        for (int i = 0; i < stepCount; i++)
        {
            var stepMesh = _arch.GenerateStep(8 - i * 0.5, 1, 6 - i * 0.3);
            var stepTransform = new TranslateTransform3D(templeX, -72 + i * 1.5, templeZ + 15 + i * 1);
            var stepMaterial = new DiffuseMaterial
            {
                Brush = new SolidColorBrush(Color.FromArgb(255, 30, 40, 55))
            };
            var stepModel = new GeometryModel3D
            {
                Geometry = stepMesh,
                Material = stepMaterial,
                BackMaterial = stepMaterial,
                Transform = stepTransform
            };
            root.Children.Add(stepModel);
        }
    }

    private void CreateMonumentalStatues(Model3DGroup root, double detail)
    {
        var statuePositions = new[]
        {
            (Pos: new Point3D(55, -68, 45), Rotation: 0.0),
            (Pos: new Point3D(145, -68, 55), Rotation: 45.0),
            (Pos: new Point3D(85, -68, 140), Rotation: 90.0),
            (Pos: new Point3D(120, -68, 35), Rotation: 135.0)
        };

        foreach (var (pos, rotation) in statuePositions)
        {
            var statue = _monuments.GenerateStatue(15, 35, (int)(12 * detail));
            var transformGroup = new Transform3DGroup();
            transformGroup.Children.Add(new TranslateTransform3D(pos.X, pos.Y, pos.Z));
            transformGroup.Children.Add(new RotateTransform3D(
                new AxisAngleRotation3D(new Vector3D(0, 1, 0), rotation)));

            var material = new DiffuseMaterial
            {
                Brush = new SolidColorBrush(Color.FromArgb(255, 35, 45, 60))
            };

            var model = new GeometryModel3D
            {
                Geometry = statue,
                Material = material,
                Transform = transformGroup
            };

            root.Children.Add(model);
        }
    }

    private void CreateAvenueOfColumns(Model3DGroup root, double detail)
    {
        var columnCount = (int)(16 * detail);
        var startX = 60;
        var startZ = 80;
        var spacing = 8;

        for (int i = 0; i < columnCount; i++)
        {
            var side = i % 2 == 0 ? -1 : 1;
            var x = startX + (i / 2) * spacing;
            var z = startZ + side * 12;

            var column = _arch.GenerateColumn(3, 25, 8, false);
            var transform = new TranslateTransform3D(x, -68, z);

            var material = new DiffuseMaterial
            {
                Brush = new SolidColorBrush(Color.FromArgb(255, 38, 48, 63))
            };

            var model = new GeometryModel3D
            {
                Geometry = column,
                Material = material,
                Transform = transform
            };

            root.Children.Add(model);
        }
    }

    private void CreateAncientBridges(Model3DGroup root, double detail)
    {
        var bridgePositions = new[]
        {
            (Start: new Point3D(50, -60, 50), End: new Point3D(70, -60, 70)),
            (Start: new Point3D(80, -58, 60), End: new Point3D(100, -58, 80)),
            (Start: new Point3D(90, -62, 40), End: new Point3D(110, -62, 60))
        };

        foreach (var (start, end) in bridgePositions)
        {
            var bridge = _arch.GenerateBridge(start, end, 4, 2);
            var material = new DiffuseMaterial
            {
                Brush = new SolidColorBrush(Color.FromArgb(255, 30, 40, 55))
            };

            var model = new GeometryModel3D
            {
                Geometry = bridge,
                Material = material,
                BackMaterial = material
            };

            root.Children.Add(model);
        }
    }

    private void CreateArchways(Model3DGroup root, double detail)
    {
        var archPositions = new[]
        {
            new Point3D(45, -68, 60),
            new Point3D(75, -68, 75),
            new Point3D(95, -68, 55),
            new Point3D(110, -68, 85)
        };

        foreach (var pos in archPositions)
        {
            var arch = _arch.GenerateArchway(8, 15, 3);
            var transform = new TranslateTransform3D(pos.X, pos.Y, pos.Z);

            var material = new DiffuseMaterial
            {
                Brush = new SolidColorBrush(Color.FromArgb(255, 42, 52, 67))
            };

            var model = new GeometryModel3D
            {
                Geometry = arch,
                Material = material,
                BackMaterial = material,
                Transform = transform
            };

            root.Children.Add(model);
        }
    }

    private void CreateBioLuminescentGarden(Model3DGroup root, double detail)
    {
        var gardenCenter = new Point3D(85, -68, 85);
        var coralCount = (int)(30 * detail);

        var random = new Random(789);
        for (int i = 0; i < coralCount; i++)
        {
            var angle = random.NextDouble() * Math.PI * 2;
            var radius = random.NextDouble() * 25 + 5;
            var x = gardenCenter.X + Math.Cos(angle) * radius;
            var z = gardenCenter.Z + Math.Sin(angle) * radius;
            var y = gardenCenter.Y + random.NextDouble() * 2;

            var coralType = random.Next(0, 3);
            var scale = 0.5 + random.NextDouble() * 1.5;

            var coralMesh = coralType switch
            {
                0 => _coral.GenerateBranchingCoral(scale, (int)(6 * detail)),
                1 => _coral.GenerateCrystalFormation(scale, (int)(8 * detail)),
                _ => _coral.GenerateCoralCluster(scale, (int)(5 * detail))
            };

            var transform = new TranslateTransform3D(x, y, z);
            var rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), random.NextDouble() * 360);
            var rotateTransform = new RotateTransform3D(rotation);

            var group = new Transform3DGroup();
            group.Children.Add(transform);
            group.Children.Add(rotateTransform);

            var glowIntensity = 0.3 + random.NextDouble() * 0.7;
            var material = new DiffuseMaterial
            {
                Brush = new SolidColorBrush(Color.FromArgb(255,
                    (byte)(20 + glowIntensity * 40),
                    (byte)(80 + glowIntensity * 80),
                    (byte)(100 + glowIntensity * 100)))
            };

            var emissive = new EmissiveMaterial
            {
                Brush = new SolidColorBrush(Color.FromArgb((byte)(glowIntensity * 100),
                    0, 200, 180))
            };

            var model = new GeometryModel3D
            {
                Geometry = coralMesh,
                Material = material,
                BackMaterial = material,
                Transform = group
            };

            root.Children.Add(model);
        }
    }

    private void CreateBrokenObservatory(Model3DGroup root, double detail)
    {
        var obsX = 95;
        var obsZ = 95;

        var basePlatform = _arch.GenerateCircularPlatform(20, 3, (int)(16 * detail));
        var baseTransform = new TranslateTransform3D(obsX, -68, obsZ);
        var baseMaterial = new DiffuseMaterial
        {
            Brush = new SolidColorBrush(Color.FromArgb(255, 45, 55, 70))
        };
        var baseModel = new GeometryModel3D
        {
            Geometry = basePlatform,
            Material = baseMaterial,
            BackMaterial = baseMaterial,
            Transform = baseTransform
        };
        root.Children.Add(baseModel);

        var ringSegments = _arch.GenerateRingSegments(12, 1.5, 2, 8);
        var ringTransform = new TranslateTransform3D(obsX, -63, obsZ);
        var ringMaterial = new DiffuseMaterial
        {
            Brush = new SolidColorBrush(Color.FromArgb(255, 50, 60, 75))
        };
        var ringModel = new GeometryModel3D
        {
            Geometry = ringSegments,
            Material = ringMaterial,
            BackMaterial = ringMaterial,
            Transform = ringTransform
        };
        root.Children.Add(ringModel);

        for (int i = 0; i < 6; i++)
        {
            var angle = i * Math.PI / 3;
            var pillarX = obsX + Math.Cos(angle) * 15;
            var pillarZ = obsZ + Math.Sin(angle) * 15;

            var pillar = _arch.GeneratePillar(1.5, 12, 4);
            var pillarTransform = new TranslateTransform3D(pillarX, -68, pillarZ);

            var pillarMaterial = new DiffuseMaterial
            {
                Brush = new SolidColorBrush(Color.FromArgb(255, 40, 50, 65))
            };

            var pillarModel = new GeometryModel3D
            {
                Geometry = pillar,
                Material = pillarMaterial,
                BackMaterial = pillarMaterial,
                Transform = pillarTransform
            };

            root.Children.Add(pillarModel);
        }
    }

    private void CreateCityDetails(Model3DGroup root, double detail)
    {
        var random = new Random(456);

        var debrisCount = (int)(40 * detail);
        for (int i = 0; i < debrisCount; i++)
        {
            var x = random.NextDouble() * 180 - 10;
            var z = random.NextDouble() * 180 - 10;
            var y = -69 + random.NextDouble() * 3;

            var size = 0.5 + random.NextDouble() * 2;
            var debris = _terrain.GenerateIrregularRock(size, size * 1.5, (int)(4 * detail));
            var transform = new TranslateTransform3D(x, y, z);

            var material = new DiffuseMaterial
            {
                Brush = new SolidColorBrush(Color.FromArgb(255,
                    (byte)(25 + random.NextDouble() * 15),
                    (byte)(35 + random.NextDouble() * 15),
                    (byte)(50 + random.NextDouble() * 15)))
            };

            var model = new GeometryModel3D
            {
                Geometry = debris,
                Material = material,
                Transform = transform
            };

            root.Children.Add(model);
        }

        for (int i = 0; i < 8; i++)
        {
            var x = random.NextDouble() * 160 + 20;
            var z = random.NextDouble() * 160 + 20;
            var y = -68;

            var carvedStone = _arch.GenerateCarvedStoneBlock(3, 1.5, 2);
            var transform = new TranslateTransform3D(x, y, z);

            var rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), random.NextDouble() * 360);
            var rotateTransform = new RotateTransform3D(rotation);

            var group = new Transform3DGroup();
            group.Children.Add(transform);
            group.Children.Add(rotateTransform);

            var material = new DiffuseMaterial
            {
                Brush = new SolidColorBrush(Color.FromArgb(255, 35, 45, 60))
            };

            var model = new GeometryModel3D
            {
                Geometry = carvedStone,
                Material = material,
                Transform = group
            };

            root.Children.Add(model);
        }
    }
}
