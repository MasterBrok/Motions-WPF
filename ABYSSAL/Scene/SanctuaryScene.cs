using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using ABYSSAL.Geometry;
using ABYSSAL.Animation;

namespace ABYSSAL.Scene;

public class SanctuaryScene
{
    public Point3D GatePosition { get; } = new(100, -65, 100);

    private readonly RingFactory _ringFactory = new();
    private readonly List<Model3D> _ringModels = new();
    private readonly List<double> _ringRotations = new();
    private readonly List<double> _ringSpeeds = new();
    private readonly List<TranslateTransform3D> _ringTranslations = new();
    private readonly List<TranslateTransform3D> _panelTranslations = new();

    private bool _isActivated;
    private double _activationProgress;
    private Transform3DGroup? _ringTransformGroup;

    public void Build(Model3DGroup root, double detail)
    {
        CreateGatePlatform(root, detail);
        CreateGateFrame(root, detail);
        CreateConcentricRings(root, detail);
        CreateGatePanels(root, detail);
        CreateCentralSeal(root, detail);
        CreateGateDecorations(root, detail);
        CreateInteriorChamber(root, detail);
    }

    private void CreateGatePlatform(Model3DGroup root, double detail)
    {
        var platform = CreateGeometry.CreateBox(60, 4, 40);
        var transform = new TranslateTransform3D(100, -72, 100);

        var material = new DiffuseMaterial
        {
            Brush = new LinearGradientBrush(
                Color.FromArgb(255, 30, 40, 55),
                Color.FromArgb(255, 20, 30, 45),
                90)
        };

        var model = new GeometryModel3D
        {
            Geometry = platform,
            Material = material,
            Transform = transform
        };

        root.Children.Add(model);
    }

    private void CreateGateFrame(Model3DGroup root, double detail)
    {
        var frameHeight = 45.0;
        var frameWidth = 35.0;

        var leftPillar = CreateGeometry.CreateBox(5, frameHeight, 5);
        var leftTransform = new TranslateTransform3D(100 - frameWidth / 2 - 2, -65 + frameHeight / 2, 100);
        var pillarMaterial = new DiffuseMaterial
        {
            Brush = new LinearGradientBrush(
                Color.FromArgb(255, 40, 50, 65),
                Color.FromArgb(255, 25, 35, 50),
                0)
        };
        var leftPillarModel = new GeometryModel3D
        {
            Geometry = leftPillar,
            Material = pillarMaterial,
            Transform = leftTransform
        };
        root.Children.Add(leftPillarModel);

        var rightPillar = CreateGeometry.CreateBox(5, frameHeight, 5);
        var rightTransform = new TranslateTransform3D(100 + frameWidth / 2 + 2, -65 + frameHeight / 2, 100);
        var rightPillarModel = new GeometryModel3D
        {
            Geometry = rightPillar,
            Material = pillarMaterial,
            Transform = rightTransform
        };
        root.Children.Add(rightPillarModel);

        var archBeam = CreateGeometry.CreateBox(frameWidth + 14, 5, 5);
        var archTransform = new TranslateTransform3D(100, -65 + frameHeight + 2, 100);
        var archMaterial = new DiffuseMaterial
        {
            Brush = new LinearGradientBrush(
                Color.FromArgb(255, 45, 55, 70),
                Color.FromArgb(255, 35, 45, 60),
                0)
        };
        var archBeamModel = new GeometryModel3D
        {
            Geometry = archBeam,
            Material = archMaterial,
            Transform = archTransform
        };
        root.Children.Add(archBeamModel);

        CreatePillarDecorations(root, frameHeight, frameWidth);
    }

    private void CreatePillarDecorations(Model3DGroup root, double height, double width)
    {
        var decorCount = 5;
        var spacing = height / (decorCount + 1);

        for (int i = 0; i < decorCount; i++)
        {
            var y = -65 + spacing * (i + 1);

            var leftDecor = CreateGeometry.CreateBox(0.5, spacing * 0.8, 5.5);
            var leftTransform = new TranslateTransform3D(100 - width / 2 - 2.3, y, 100);
            var decorMaterial = new DiffuseMaterial
            {
                Brush = new SolidColorBrush(Color.FromArgb(255, 35, 45, 60))
            };
            var leftDecorModel = new GeometryModel3D
            {
                Geometry = leftDecor,
                Material = decorMaterial,
                Transform = leftTransform
            };
            root.Children.Add(leftDecorModel);

            var rightDecor = CreateGeometry.CreateBox(0.5, spacing * 0.8, 5.5);
            var rightTransform = new TranslateTransform3D(100 + width / 2 + 2.3, y, 100);
            var rightDecorModel = new GeometryModel3D
            {
                Geometry = rightDecor,
                Material = decorMaterial,
                Transform = rightTransform
            };
            root.Children.Add(rightDecorModel);
        }
    }

    private void CreateConcentricRings(Model3DGroup root, double detail)
    {
        _ringTransformGroup = new Transform3DGroup();
        _ringTransformGroup.Children.Add(new TranslateTransform3D(100, -52, 100));

        var ringCount = 5;
        for (int i = 0; i < ringCount; i++)
        {
            var radius = 12 - i * 2;
            var thickness = 1.5 - i * 0.15;
            var segments = (int)(24 - i * 2);

            var ring = _ringFactory.GenerateRing(radius, thickness, segments);

            var rotation = i * 15.0;
            var rotationTransform = new RotateTransform3D(
                new AxisAngleRotation3D(new Vector3D(0, 0, 1), rotation));

            var group = new Transform3DGroup();
            group.Children.Add(rotationTransform);

            var ringMaterial = new DiffuseMaterial
            {
                Brush = new LinearGradientBrush(
                    Color.FromArgb(255, 50, 60, 75),
                    Color.FromArgb(255, 35, 45, 60),
                    0)
            };

            var model = new GeometryModel3D
            {
                Geometry = ring,
                Material = ringMaterial,
                Transform = group
            };

            _ringModels.Add(model);
            _ringRotations.Add(rotation);
            _ringSpeeds.Add(0.02 + i * 0.005);

            root.Children.Add(model);
        }
    }

    private void CreateGatePanels(Model3DGroup root, double detail)
    {
        var panelCount = 8;
        for (int i = 0; i < panelCount; i++)
        {
            var angle = i * (360.0 / panelCount);
            var radius = 14;
            var x = 100 + Math.Cos(angle * Math.PI / 180) * radius;
            var z = 100 + Math.Sin(angle * Math.PI / 180) * radius;

            var panel = CreateGeometry.CreateBox(3, 8, 1);
            var translation = new TranslateTransform3D(x, -52, z);

            var rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), angle);
            var rotationTransform = new RotateTransform3D(rotation);

            var group = new Transform3DGroup();
            group.Children.Add(translation);
            group.Children.Add(rotationTransform);

            var panelMaterial = new DiffuseMaterial
            {
                Brush = new SolidColorBrush(Color.FromArgb(255, 40, 50, 65))
            };

            var model = new GeometryModel3D
            {
                Geometry = panel,
                Material = panelMaterial,
                Transform = group
            };

            _panelTranslations.Add(translation);

            root.Children.Add(model);
        }
    }

    private void CreateCentralSeal(Model3DGroup root, double detail)
    {
        var sealOuter = _ringFactory.GenerateRing(6, 1, 32);
        var sealTransform = new TranslateTransform3D(100, -52, 100);

        var sealMaterial = new DiffuseMaterial
        {
            Brush = new LinearGradientBrush(
                Color.FromArgb(255, 45, 55, 70),
                Color.FromArgb(255, 30, 40, 55),
                0)
        };

        var sealModel = new GeometryModel3D
        {
            Geometry = sealOuter,
            Material = sealMaterial,
            Transform = sealTransform
        };

        root.Children.Add(sealModel);

        var sealInner = _ringFactory.GenerateRing(4, 0.8, 24);
        var innerTransform = new TranslateTransform3D(100, -52, 100);

        var innerMaterial = new DiffuseMaterial
        {
            Brush = new LinearGradientBrush(
                Color.FromArgb(255, 55, 65, 80),
                Color.FromArgb(255, 40, 50, 65),
                0)
        };

        var innerModel = new GeometryModel3D
        {
            Geometry = sealInner,
            Material = innerMaterial,
            Transform = innerTransform
        };

        root.Children.Add(innerModel);

        var coreSeal = _ringFactory.GenerateDisc(3, 16);
        var coreTransform = new TranslateTransform3D(100, -52, 100);

        var coreMaterial = new EmissiveMaterial
        {
            Brush = new RadialGradientBrush(
                Color.FromArgb(200, 255, 215, 0),
                Color.FromArgb(50, 150, 120, 0))
        };

        var coreModel = new GeometryModel3D
        {
            Geometry = coreSeal,
            Material = coreMaterial,
            BackMaterial = coreMaterial,
            Transform = coreTransform
        };

        root.Children.Add(coreModel);
    }

    private void CreateGateDecorations(Model3DGroup root, double detail)
    {
        var symbolPositions = new[]
        {
            new Point3D(100 - 18, -40, 100),
            new Point3D(100 + 18, -40, 100),
            new Point3D(100, -40, 100 - 18),
            new Point3D(100, -40, 100 + 18)
        };

        foreach (var pos in symbolPositions)
        {
            var symbol = CreateGeometry.CreateBox(2, 4, 0.5);

            var rotationAxis = pos.X == 100 ? new Vector3D(1, 0, 0) : new Vector3D(0, 0, 1);
            var transformGroup = new Transform3DGroup();
            transformGroup.Children.Add(new TranslateTransform3D(pos.X, pos.Y, pos.Z));
            transformGroup.Children.Add(new RotateTransform3D(
                new AxisAngleRotation3D(rotationAxis, 90)));

            var symbolMaterial = new EmissiveMaterial
            {
                Brush = new SolidColorBrush(Color.FromArgb(150, 255, 200, 0))
            };

            var symbolModel = new GeometryModel3D
            {
                Geometry = symbol,
                Material = symbolMaterial,
                BackMaterial = symbolMaterial,
                Transform = transformGroup
            };

            root.Children.Add(symbolModel);
        }
    }

    private void CreateInteriorChamber(Model3DGroup root, double detail)
    {
        var chamberFloor = CreateGeometry.CreateBox(25, 2, 20);
        var floorTransform = new TranslateTransform3D(100, -74, 115);

        var floorMaterial = new DiffuseMaterial
        {
            Brush = new LinearGradientBrush(
                Color.FromArgb(255, 50, 60, 75),
                Color.FromArgb(255, 35, 45, 60),
                90)
        };

        var floorModel = new GeometryModel3D
        {
            Geometry = chamberFloor,
            Material = floorMaterial,
            Transform = floorTransform
        };

        root.Children.Add(floorModel);

        var energySource = _ringFactory.GenerateCrystal(3, (int)(12 * detail));
        var energyTransform = new TranslateTransform3D(100, -65, 120);

        var energyMaterial = new EmissiveMaterial
        {
            Brush = new RadialGradientBrush(
                Color.FromArgb(255, 255, 240, 100),
                Color.FromArgb(50, 180, 150, 0))
        };

        var energyModel = new GeometryModel3D
        {
            Geometry = energySource,
            Material = energyMaterial,
            BackMaterial = energyMaterial,
            Transform = energyTransform
        };

        root.Children.Add(energyModel);
    }

    public void InitializeAnimations(TimelineController timeline)
    {
    }

    public void Update(TimeSpan time)
    {
        if (time.TotalSeconds >= 230)
        {
            _isActivated = true;
            _activationProgress = Math.Min((time.TotalSeconds - 230) / 30.0, 1.0);
            AnimateGateOpening();
        }
        else if (time.TotalSeconds >= 200)
        {
            AnimateDormantPulse(time);
        }
    }

    private void AnimateDormantPulse(TimeSpan time)
    {
        var pulse = (Math.Sin(time.TotalSeconds * 2) + 1) / 2;
        var intensity = (byte)(80 + pulse * 40);
    }

    private void AnimateGateOpening()
    {
        for (int i = 0; i < _ringModels.Count; i++)
        {
            if (_ringModels[i].Transform is Transform3DGroup group)
            {
                var newRotation = _ringRotations[i] + _activationProgress * 360 * _ringSpeeds[i] * 100;
                group.Children.Clear();
                group.Children.Add(new RotateTransform3D(
                    new AxisAngleRotation3D(new Vector3D(0, 0, 1), newRotation)));
            }
        }
    }

    public bool IsActivated => _isActivated;
    public double ActivationProgress => _activationProgress;
}