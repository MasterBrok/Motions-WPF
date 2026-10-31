using System.Windows.Media.Media3D;
using ABYSSAL.Scene;
using ABYSSAL.Geometry;
using ABYSSAL.Effects;
using ABYSSAL.Materials;

namespace ABYSSAL;

public class SceneManager
{
    public LightingRig Lighting { get; private set; }
    private readonly OceanScene _oceanScene;
    private readonly TrenchScene _trenchScene;
    private readonly LostCityScene _lostCityScene;
    private readonly SanctuaryScene _sanctuaryScene;
    private readonly ParticleSystem3D _particleSystem;
    private readonly BubbleSystem _bubbleSystem;
    private readonly BioluminescenceSystem _biolumSystem;
    private readonly CreatureSystem _creatureSystem;
    private Model3DGroup _sceneRoot = new();
    private QualityLevel _currentQuality = QualityLevel.Balanced;

    public SceneManager()
    {
        Lighting = new LightingRig();
        _oceanScene = new OceanScene();
        _trenchScene = new TrenchScene();
        _lostCityScene = new LostCityScene();
        _sanctuaryScene = new SanctuaryScene();
        _particleSystem = new ParticleSystem3D();
        _bubbleSystem = new BubbleSystem();
        _biolumSystem = new BioluminescenceSystem();
        _creatureSystem = new CreatureSystem();
    }

    public void BuildScene(Model3DGroup root, QualityLevel quality)
    {
        _sceneRoot = root;
        _currentQuality = quality;

        var geometryDetail = quality switch
        {
            QualityLevel.Performance => 0.5,
            QualityLevel.Balanced => 0.75,
            QualityLevel.Cinematic => 1.0,
            _ => 0.75
        };

        _oceanScene.Build(root, 0.0);
        _trenchScene.Build(root, geometryDetail);
        _lostCityScene.Build(root, geometryDetail);
        _sanctuaryScene.Build(root, geometryDetail);

        var particleCount = quality switch
        {
            QualityLevel.Performance => 500,
            QualityLevel.Balanced => 1500,
            QualityLevel.Cinematic => 3000,
            _ => 1500
        };
        _particleSystem.Initialize(root, particleCount);

        _bubbleSystem.Initialize(root, quality);
        _biolumSystem.Initialize(root, quality);

        var creatureCount = quality switch
        {
            QualityLevel.Performance => 20,
            QualityLevel.Balanced => 50,
            QualityLevel.Cinematic => 100,
            _ => 50
        };
        _creatureSystem.Initialize(root, creatureCount, quality);
    }

    public void InitializeAnimations(TimelineController timeline)
    {
        _sanctuaryScene.InitializeAnimations(timeline);
        _biolumSystem.InitializeAnimations(timeline);
        _creatureSystem.InitializeAnimations(timeline);
    }

    public void Update(TimeSpan time, QualityLevel quality)
    {
        if (quality != _currentQuality)
        {
            _currentQuality = quality;
        }

        _particleSystem.Update(time);
        _bubbleSystem.Update(time);
        _biolumSystem.Update(time, quality);
        _sanctuaryScene.Update(time);

        Lighting.Update(time);
    }

    public void UpdateCreatures(TimeSpan time)
    {
        _creatureSystem.Update(time);
    }

    public void UpdateQuality(QualityLevel level)
    {
        _currentQuality = level;
    }

    public Point3D GetSanctuaryPosition() => _sanctuaryScene.GatePosition;
    public Model3DGroup GetSceneRoot() => _sceneRoot;
}
