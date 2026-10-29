using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using AnimeEclipse.Animation;

namespace AnimeEclipse
{
    public partial class MainWindow : Window
    {
        private readonly TimelineManager _timeline;
        private readonly SceneRenderer _sceneRenderer;
        private readonly ParticleSystem _particleSystem;
        private readonly SakuraParticleSystem _sakuraSystem;
        private DateTime _startTime;
        private readonly DispatcherTimer _renderTimer;

        public MainWindow()
        {
            InitializeComponent();
            _timeline = new TimelineManager(30000);
            _sceneRenderer = new SceneRenderer();
            _particleSystem = new ParticleSystem(ParticleLayer);
            _sakuraSystem = new SakuraParticleSystem(ParticleLayer);

            _renderTimer = new DispatcherTimer(DispatcherPriority.Render)
            {
                Interval = TimeSpan.FromMilliseconds(16)
            };
            _renderTimer.Tick += OnRenderTick;
            Loaded += OnLoaded;
            Closing += OnClosing;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _sceneRenderer.Initialize(this);
            _startTime = DateTime.Now;
            _renderTimer.Start();
        }

        private void OnClosing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            _renderTimer.Stop();
            _particleSystem.Dispose();
            _sakuraSystem.Dispose();
        }

        private void OnRenderTick(object? sender, EventArgs e)
        {
            var elapsed = (DateTime.Now - _startTime).TotalMilliseconds;
            var progress = Math.Min(elapsed / 30000.0, 1.0);

            _timeline.Update(progress);
            _sceneRenderer.Render(_timeline);
            _particleSystem.Update(_timeline);
            _sakuraSystem.Update(_timeline);
            UpdateOverlays(_timeline);

            if (progress >= 1.0)
            {
                _renderTimer.Stop();
            }
        }

        private void UpdateOverlays(TimelineManager timeline)
        {
            FlashOverlay.Opacity = timeline.FlashIntensity;
            VignetteOverlay.Opacity = 0.6 + timeline.VignetteIntensity * 0.2;
        }

        public Canvas GetBackgroundLayer() => BackgroundLayer;
        public Canvas GetMidgroundLayer() => MidgroundLayer;
        public Canvas GetForegroundLayer() => ForegroundLayer;
        public Canvas GetEffectLayer() => EffectLayer;
        public Canvas GetTypographyLayer() => TypographyLayer;
    }
}
