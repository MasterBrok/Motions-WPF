using System.Windows.Media.Media3D;
using Chronos.Geometry;
using Chronos.Materials;

namespace Chronos.Models;

public class MechanicalHeart
{
    private readonly Model3DGroup _ringsGroup = new();
    private readonly Model3DGroup _gearsGroup = new();
    private readonly Model3DGroup _platesGroup = new();
    private readonly Model3DGroup _coreGroup = new();
    private readonly Model3DGroup _armsGroup = new();

    public Model3DGroup Rings => _ringsGroup;
    public Model3DGroup Gears => _gearsGroup;
    public Model3DGroup Plates => _platesGroup;
    public Model3DGroup Core => _coreGroup;
    public Model3DGroup Arms => _armsGroup;

    public RotateTransform3D RingsRotation { get; } = new(new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0));
    public RotateTransform3D GearsRotation { get; } = new(new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0));
    public RotateTransform3D PlatesRotation { get; } = new(new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0));
    public ScaleTransform3D CoreScaleTransform { get; } = new(1, 1, 1);
    public TranslateTransform3D RingsOffset { get; } = new();
    public TranslateTransform3D PlatesOffset { get; } = new();

    public AxisAngleRotation3D RingsAngle => (AxisAngleRotation3D)RingsRotation.Rotation;
    public AxisAngleRotation3D GearsAngle => (AxisAngleRotation3D)GearsRotation.Rotation;
    public AxisAngleRotation3D PlatesAngle => (AxisAngleRotation3D)PlatesRotation.Rotation;

    private readonly List<GeometryModel3D> _ringModels = new();
    private readonly List<GeometryModel3D> _gearModels = new();
    private readonly List<GeometryModel3D> _plateModels = new();
    private GeometryModel3D? _coreModel;

    public IReadOnlyList<GeometryModel3D> RingModels => _ringModels;
    public IReadOnlyList<GeometryModel3D> GearModels => _gearModels;
    public IReadOnlyList<GeometryModel3D> PlateModels => _plateModels;
    public GeometryModel3D? CoreModel => _coreModel;

    public void CreateHeart(Model3DGroup parent)
    {
        CreateRings();
        CreateGears();
        CreatePlates();
        CreateCore();
        CreateArms();

        var ringsT = new Transform3DGroup();
        ringsT.Children.Add(RingsRotation);
        ringsT.Children.Add(RingsOffset);
        _ringsGroup.Transform = ringsT;

        _gearsGroup.Transform = GearsRotation;

        var platesT = new Transform3DGroup();
        platesT.Children.Add(PlatesRotation);
        platesT.Children.Add(PlatesOffset);
        _platesGroup.Transform = platesT;

        _coreGroup.Transform = CoreScaleTransform;

        parent.Children.Add(_ringsGroup);
        parent.Children.Add(_gearsGroup);
        parent.Children.Add(_platesGroup);
        parent.Children.Add(_coreGroup);
        parent.Children.Add(_armsGroup);
    }

    private void CreateRings()
    {
        double[][] ringConfigs = new double[][]
        {
            new double[] { 2.0, 2.4, 0.15 },
            new double[] { 2.6, 2.9, 0.12 },
            new double[] { 3.2, 3.4, 0.10 },
        };

        for (int i = 0; i < ringConfigs.Length; i++)
        {
            var cfg = ringConfigs[i];
            var mesh = RingMeshFactory.Create(cfg[0], cfg[1], cfg[2], 64);
            var material = i % 2 == 0
                ? MaterialLibrary.CreateBlackTitanium()
                : MaterialLibrary.CreateBrushedGold();

            var model = new GeometryModel3D
            {
                Geometry = mesh,
                Material = material,
                BackMaterial = material
            };
            model.Freeze();
            _ringModels.Add(model);
            _ringsGroup.Children.Add(model);
        }
    }

    private void CreateGears()
    {
        var gearConfigs = new (int teeth, double inner, double outer, double thickness)[]
        {
            (16, 0.8, 1.2, 0.15),
            (12, 0.6, 1.0, 0.15),
            (20, 1.0, 1.4, 0.12),
        };

        for (int i = 0; i < gearConfigs.Length; i++)
        {
            var cfg = gearConfigs[i];
            var mesh = GearMeshFactory.Create(cfg.teeth, cfg.inner, cfg.outer, cfg.thickness, 0.3, 24);
            var model = new GeometryModel3D
            {
                Geometry = mesh,
                Material = MaterialLibrary.CreatePolishedChrome(),
                BackMaterial = MaterialLibrary.CreatePolishedChrome()
            };
            model.Freeze();
            _gearModels.Add(model);
            _gearsGroup.Children.Add(model);
        }
    }

    private void CreatePlates()
    {
        for (int i = 0; i < 8; i++)
        {
            var mesh = GearMeshFactory.CreateMechanicalPlate(0.8, 1.2, 0.08);
            var model = new GeometryModel3D
            {
                Geometry = mesh,
                Material = MaterialLibrary.CreateBlackTitanium(),
                BackMaterial = MaterialLibrary.CreatePolishedChrome()
            };
            model.Freeze();
            _plateModels.Add(model);
            _platesGroup.Children.Add(model);
        }
    }

    private void CreateCore()
    {
        var mesh = GearMeshFactory.CreateEnergyCore(0.6, 20);
        _coreModel = new GeometryModel3D
        {
            Geometry = mesh,
            Material = MaterialLibrary.CreateCyanCore(),
            BackMaterial = MaterialLibrary.CreateCyanCore()
        };
        _coreModel.Freeze();
        _coreGroup.Children.Add(_coreModel);
    }

    private void CreateArms()
    {
        var armMesh = CylinderMeshFactory.Create(0.08, 2.0, 16);
        for (int i = 0; i < 4; i++)
        {
            var model = new GeometryModel3D
            {
                Geometry = armMesh,
                Material = MaterialLibrary.CreatePolishedChrome()
            };
            model.Freeze();
            _armsGroup.Children.Add(model);
        }
    }
}