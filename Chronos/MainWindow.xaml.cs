using System.Windows;
using Chronos.Scene;
using Chronos.Animation;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Chronos;

public partial class MainWindow : Window
{
    private readonly SceneBuilder _sceneBuilder;
    private readonly TimelineController _timeline;
    private MechanicalAnimator _animator = null!;
    private readonly CameraRig _cameraRig;
    private System.Windows.Threading.DispatcherTimer? _renderTimer;
    private DateTime _startTime;
    private TimeSpan _pausedElapsed;
    private bool _isPlaying;

    public MainWindow()
    {
        InitializeComponent();

        _sceneBuilder = new SceneBuilder();
        _cameraRig = new CameraRig();
        _timeline = new TimelineController();

        SetupViewport();
        SetupControls();

        _timeline.SeekCompleted += OnSeekCompleted;
        Loaded += (s, e) => Play();
    }

    private void SetupViewport()
    {
        _cameraRig.Initialize(_sceneBuilder.GetHeartCenter());
        Viewport.Children.Clear();
        Viewport.Camera = _cameraRig.Camera;

        var modelGroup = new Model3DGroup();
        _sceneBuilder.BuildScene(modelGroup);

        foreach (var light in _sceneBuilder.Lighting.GetAllLights())
        {
            modelGroup.Children.Add(light);
        }

        var modelVisual = new ModelVisual3D();
        modelVisual.Content = modelGroup;
        Viewport.Children.Add(modelVisual);

        _animator = new MechanicalAnimator(_timeline, _sceneBuilder.Heart);
    }

    private void SetupControls()
    {
        PlayButton.Click += (s, e) =>
        {
            if (_isPlaying) Pause();
            else Play();
        };

        RestartButton.Click += (s, e) => Restart();
        Scene1Button.Click += (s, e) => SeekToScene(0);
        Scene2Button.Click += (s, e) => SeekToScene(5);
        Scene3Button.Click += (s, e) => SeekToScene(10);
        Scene4Button.Click += (s, e) => SeekToScene(16);
        Scene5Button.Click += (s, e) => SeekToScene(22);
        Scene6Button.Click += (s, e) => SeekToScene(27);

        QualityLow.Click += (s, e) => SetQuality(QualityLevel.Low);
        QualityMedium.Click += (s, e) => SetQuality(QualityLevel.Medium);
        QualityHigh.Click += (s, e) => SetQuality(QualityLevel.High);
    }

    private void Play()
    {
        if (_isPlaying) return;
        _isPlaying = true;
        _startTime = DateTime.Now - _pausedElapsed;
        PlayButton.Content = "Pause";

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
        PlayButton.Content = "Play";
        _renderTimer?.Stop();
    }

    private void Restart()
    {
        Pause();
        _pausedElapsed = TimeSpan.Zero;
        _timeline.Seek(TimeSpan.Zero);
    }

    private void SeekToScene(double seconds)
    {
        Pause();
        _timeline.Seek(TimeSpan.FromSeconds(seconds));
        _pausedElapsed = TimeSpan.FromSeconds(seconds);
    }

    private void SetQuality(QualityLevel level)
    {
        _animator.SetQuality(level);
    }

    private void OnRenderTick(object? sender, EventArgs e)
    {
        var elapsed = DateTime.Now - _startTime;
        var totalDuration = TimeSpan.FromSeconds(30);

        if (elapsed >= totalDuration)
        {
            Pause();
            elapsed = totalDuration;
            PlayButton.Content = "Play";
        }

        _timeline.Update(elapsed);
        _cameraRig.Update(_timeline.CurrentTime);
        _animator.Update();

        TitleText.Text = $"CHRONOS - {elapsed.TotalSeconds:F1}s / 30s";

        var progress = elapsed.TotalSeconds / 30.0;
        ProgressBar.Value = progress * 100;

        double seconds = elapsed.TotalSeconds;
        FadeOverlay.Opacity = seconds > 29.0 ? (seconds - 29.0) * 2.0 : 0;
    }

    private void OnSeekCompleted(object? sender, EventArgs e)
    {
        _cameraRig.SnapTo(_timeline.CurrentTime.TotalSeconds);
        _animator?.Update();
        FadeOverlay.Opacity = 0;
    }
}

public enum QualityLevel { Low, Medium, High }