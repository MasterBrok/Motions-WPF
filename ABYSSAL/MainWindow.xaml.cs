using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ABYSSAL;

public partial class MainWindow : Window
{
    private readonly SceneManager _sceneManager;
    private readonly TimelineController _timeline;
    private readonly CinematicCameraRig _cameraRig;
    private System.Windows.Threading.DispatcherTimer? _renderTimer;
    private DateTime _startTime;
    private TimeSpan _pausedElapsed;
    private bool _isPlaying;
    private QualityLevel _quality = QualityLevel.Balanced;

    public MainWindow()
    {
        InitializeComponent();
        _sceneManager = new SceneManager();
        _cameraRig = new CinematicCameraRig();
        _timeline = new TimelineController();

        SetupViewport();
        SetupControls();
        BuildSequence();

        _timeline.SeekCompleted += OnSeekCompleted;
        Loaded += (s, e) => Play();
    }

    private void SetupViewport()
    {
        _cameraRig.Initialize(new Point3D(0, 120, 0), new Vector3D(0, -1, 0));
        Viewport.Children.Clear();
        Viewport.Camera = _cameraRig.Camera;

        var modelGroup = new Model3DGroup();
        _sceneManager.BuildScene(modelGroup, _quality);

        foreach (var light in _sceneManager.Lighting.GetAllLights())
        {
            modelGroup.Children.Add(light);
        }

        var modelVisual = new ModelVisual3D();
        modelVisual.Content = modelGroup;
        Viewport.Children.Add(modelVisual);

        _sceneManager.InitializeAnimations(_timeline);
    }

    private void SetupControls()
    {
        PlayButton.Click += (s, e) =>
        {
            if (_isPlaying) Pause();
            else Play();
        };

        RestartButton.Click += (s, e) => Restart();
        TimelineSlider.ValueChanged += (s, e) =>
        {
            if (!_isPlaying)
            {
                var time = TimeSpan.FromSeconds(TimelineSlider.Value);
                _timeline.Seek(time);
                UpdateSceneState(time);
            }
        };

        Scene1Button.Click += (s, e) => SeekToScene("01 - Above the Unknown", 0);
        Scene2Button.Click += (s, e) => SeekToScene("02 - Descent into the Abyss", 25);
        Scene3Button.Click += (s, e) => SeekToScene("03 - The First Relic", 50);
        Scene4Button.Click += (s, e) => SeekToScene("04 - The Lost City Revealed", 75);
        Scene5Button.Click += (s, e) => SeekToScene("05 - The Avenue of Giants", 100);
        Scene6Button.Click += (s, e) => SeekToScene("06 - The Bio-Luminescent Garden", 125);
        Scene7Button.Click += (s, e) => SeekToScene("07 - The Broken Observatory", 150);
        Scene8Button.Click += (s, e) => SeekToScene("08 - Approach to the Sanctuary", 175);
        Scene9Button.Click += (s, e) => SeekToScene("09 - The Dormant Symbol", 200);
        Scene10Button.Click += (s, e) => SeekToScene("10 - The Gate Awakens", 230);
        Scene11Button.Click += (s, e) => SeekToScene("11 - The Light Beyond", 260);
        Scene12Button.Click += (s, e) => SeekToScene("12 - The Final Revelation", 295);

        QualityLow.Click += (s, e) => SetQuality(QualityLevel.Performance);
        QualityMedium.Click += (s, e) => SetQuality(QualityLevel.Balanced);
        QualityHigh.Click += (s, e) => SetQuality(QualityLevel.Cinematic);
    }

    private void BuildSequence()
    {
        _timeline.Duration = TimeSpan.FromSeconds(330);
        TimelineSlider.Maximum = 330;

        _timeline.AddShot(new CameraShot
        {
            Id = "01",
            Name = "Above the Unknown",
            StartTime = TimeSpan.FromSeconds(0),
            EndTime = TimeSpan.FromSeconds(25),
            StartPosition = new Point3D(0, 120, 0),
            EndPosition = new Point3D(0, 20, 0),
            LookTarget = new Point3D(0, 0, 0),
            TransitionType = TransitionType.Continuous
        });

        _timeline.AddShot(new CameraShot
        {
            Id = "02",
            Name = "Descent into the Abyss",
            StartTime = TimeSpan.FromSeconds(25),
            EndTime = TimeSpan.FromSeconds(50),
            StartPosition = new Point3D(0, 20, 0),
            EndPosition = new Point3D(15, -50, 20),
            LookTarget = new Point3D(0, -60, 30),
            TransitionType = TransitionType.Continuous
        });

        _timeline.AddShot(new CameraShot
        {
            Id = "03",
            Name = "The First Relic",
            StartTime = TimeSpan.FromSeconds(50),
            EndTime = TimeSpan.FromSeconds(75),
            StartPosition = new Point3D(30, -55, 30),
            EndPosition = new Point3D(20, -65, 25),
            LookTarget = new Point3D(25, -60, 20),
            TransitionType = TransitionType.Continuous
        });

        _timeline.AddShot(new CameraShot
        {
            Id = "04",
            Name = "The Lost City Revealed",
            StartTime = TimeSpan.FromSeconds(75),
            EndTime = TimeSpan.FromSeconds(100),
            StartPosition = new Point3D(40, -50, 40),
            EndPosition = new Point3D(50, -40, 50),
            LookTarget = new Point3D(100, -80, 100),
            TransitionType = TransitionType.Continuous
        });

        _timeline.AddShot(new CameraShot
        {
            Id = "05",
            Name = "The Avenue of Giants",
            StartTime = TimeSpan.FromSeconds(100),
            EndTime = TimeSpan.FromSeconds(125),
            StartPosition = new Point3D(60, -45, 60),
            EndPosition = new Point3D(80, -50, 80),
            LookTarget = new Point3D(100, -60, 100),
            TransitionType = TransitionType.Continuous
        });

        _timeline.AddShot(new CameraShot
        {
            Id = "06",
            Name = "The Bio-Luminescent Garden",
            StartTime = TimeSpan.FromSeconds(125),
            EndTime = TimeSpan.FromSeconds(150),
            StartPosition = new Point3D(70, -48, 70),
            EndPosition = new Point3D(85, -52, 85),
            LookTarget = new Point3D(90, -55, 90),
            TransitionType = TransitionType.Continuous
        });

        _timeline.AddShot(new CameraShot
        {
            Id = "07",
            Name = "The Broken Observatory",
            StartTime = TimeSpan.FromSeconds(150),
            EndTime = TimeSpan.FromSeconds(175),
            StartPosition = new Point3D(88, -50, 88),
            EndPosition = new Point3D(95, -48, 95),
            LookTarget = new Point3D(100, -55, 100),
            TransitionType = TransitionType.Continuous
        });

        _timeline.AddShot(new CameraShot
        {
            Id = "08",
            Name = "Approach to the Sanctuary",
            StartTime = TimeSpan.FromSeconds(175),
            EndTime = TimeSpan.FromSeconds(200),
            StartPosition = new Point3D(90, -50, 90),
            EndPosition = new Point3D(95, -60, 95),
            LookTarget = new Point3D(100, -65, 100),
            TransitionType = TransitionType.Continuous
        });

        _timeline.AddShot(new CameraShot
        {
            Id = "09",
            Name = "The Dormant Symbol",
            StartTime = TimeSpan.FromSeconds(200),
            EndTime = TimeSpan.FromSeconds(230),
            StartPosition = new Point3D(96, -65, 96),
            EndPosition = new Point3D(98, -63, 98),
            LookTarget = new Point3D(100, -65, 100),
            TransitionType = TransitionType.Continuous
        });

        _timeline.AddShot(new CameraShot
        {
            Id = "10",
            Name = "The Gate Awakens",
            StartTime = TimeSpan.FromSeconds(230),
            EndTime = TimeSpan.FromSeconds(260),
            StartPosition = new Point3D(97, -64, 97),
            EndPosition = new Point3D(99, -62, 99),
            LookTarget = new Point3D(100, -65, 100),
            TransitionType = TransitionType.Continuous
        });

        _timeline.AddShot(new CameraShot
        {
            Id = "11",
            Name = "The Light Beyond",
            StartTime = TimeSpan.FromSeconds(260),
            EndTime = TimeSpan.FromSeconds(295),
            StartPosition = new Point3D(99, -63, 99),
            EndPosition = new Point3D(100, -65, 105),
            LookTarget = new Point3D(100, -65, 110),
            TransitionType = TransitionType.Continuous
        });

        _timeline.AddShot(new CameraShot
        {
            Id = "12",
            Name = "The Final Revelation",
            StartTime = TimeSpan.FromSeconds(295),
            EndTime = TimeSpan.FromSeconds(330),
            StartPosition = new Point3D(100, -65, 105),
            EndPosition = new Point3D(100, -60, 100),
            LookTarget = new Point3D(100, -65, 100),
            TransitionType = TransitionType.FadeOut
        });
    }

    private void UpdateSceneState(TimeSpan time)
    {
        _sceneManager.Update(time, _quality);
        _sceneManager.UpdateCreatures(time);
    }

    private void OnSeekCompleted()
    {
    }

    private void SeekToScene(string name, double seconds)
    {
        SceneNameText.Text = name;
        TimelineSlider.Value = seconds;
        var time = TimeSpan.FromSeconds(seconds);
        _timeline.Seek(time);
        UpdateSceneState(time);
    }

    private void Play()
    {
        if (_isPlaying) return;
        _isPlaying = true;
        _startTime = DateTime.Now - _pausedElapsed;
        PlayButton.Content = "PAUSE";

        _renderTimer = new System.Windows.Threading.DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(16)
        };
        _renderTimer.Tick += OnRenderTick;
        _renderTimer.Start();
    }

    private void Pause()
    {
        if (!_isPlaying) return;
        _isPlaying = false;
        _pausedElapsed = DateTime.Now - _startTime;
        PlayButton.Content = "PLAY";
        _renderTimer?.Stop();
    }

    private void Restart()
    {
        Pause();
        _pausedElapsed = TimeSpan.Zero;
        _timeline.Restart();
        UpdateSceneState(TimeSpan.Zero);
        Play();
    }

    private void SetQuality(QualityLevel level)
    {
        _quality = level;
        _sceneManager.UpdateQuality(level);
    }

    private void OnRenderTick(object? sender, EventArgs e)
    {
        var elapsed = DateTime.Now - _startTime;
        _timeline.Update(elapsed);

        if (elapsed >= _timeline.Duration)
        {
            Restart();
            return;
        }

        var currentTime = elapsed;
        TimelineSlider.Value = currentTime.TotalSeconds;
        _sceneManager.Update(currentTime, _quality);
        _sceneManager.UpdateCreatures(currentTime);

        _cameraRig.Update(_timeline.GetCurrentShot(), currentTime);

        SceneNameText.Text = _timeline.GetCurrentShot()?.Name ?? "ABYSSAL";

        if (_timeline.GetCurrentShot()?.Id == "12" && currentTime.TotalSeconds > 320)
        {
            var fadeAmount = (currentTime.TotalSeconds - 320) / 10.0;
            FadeOverlay.Opacity = Math.Min(fadeAmount, 1.0);

            if (currentTime.TotalSeconds > 325 && TitleOverlay.Opacity < 1)
            {
                TitleOverlay.Opacity = Math.Min(TitleOverlay.Opacity + 0.05, 1);
            }
        }
        else
        {
            FadeOverlay.Opacity = 0;
        }
    }

    protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == System.Windows.Input.Key.Space)
        {
            if (_isPlaying) Pause();
            else Play();
        }
        else if (e.Key == System.Windows.Input.Key.F11)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }
    }
}

public enum QualityLevel
{
    Performance,
    Balanced,
    Cinematic
}

public enum TransitionType
{
    Continuous,
    FadeIn,
    FadeOut,
    HardCut
}
