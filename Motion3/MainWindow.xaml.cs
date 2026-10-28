using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Motion3.Animation;
using Motion3.Controls;

namespace Motion3;

public partial class MainWindow : Window
{
    private Storyboard? _timeline;
    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private double _lastFrameSeconds;
    private bool _playing;

    // FPS measurement (rolling ~0.5 s window)
    private int _frameCount;
    private double _frameAccum;
    private double _fps;

    private Point _center = new(640, 360);

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        SetupLayout();
        BuildTimeline();
        StartPlayback();
    }

    private void SetupLayout()
    {
        double cx = ActualWidth / 2.0;
        double cy = ActualHeight / 2.0;
        _center = new Point(cx, cy);

        PositionScene1(cx, cy);
        PositionScene2(cx, cy);
        PositionScene3(cx, cy);
        PositionScene4(cx, cy);
        PositionScene5(cx, cy);
    }

    private void PositionScene1(double cx, double cy)
    {
        Canvas.SetLeft(AwakenLine, cx - 300);
        Canvas.SetTop(AwakenLine, cy);
        Canvas.SetLeft(HexFrag0, cx - 190); Canvas.SetTop(HexFrag0, cy - 80);
        Canvas.SetLeft(HexFrag1, cx + 180); Canvas.SetTop(HexFrag1, cy + 60);
        Canvas.SetLeft(HexFrag2, cx - 80); Canvas.SetTop(HexFrag2, cy + 100);
        Canvas.SetLeft(HexFrag3, cx + 80); Canvas.SetTop(HexFrag3, cy - 80);
        Canvas.SetLeft(HexFrag4, cx - 140); Canvas.SetTop(HexFrag4, cy - 10);
        Canvas.SetLeft(HexFrag5, cx + 140); Canvas.SetTop(HexFrag5, cy - 20);
        Canvas.SetTop(ScanBeam, cy - 200);
        Canvas.SetLeft(Scene1Text, cx - 260);
        Canvas.SetTop(Scene1Text, cy + 120);
    }

    private void PositionScene2(double cx, double cy)
    {
        Canvas.SetLeft(P2Ring0, cx - P2Ring0.Width / 2); Canvas.SetTop(P2Ring0, cy - P2Ring0.Height / 2);
        Canvas.SetLeft(P2Ring1, cx - P2Ring1.Width / 2); Canvas.SetTop(P2Ring1, cy - P2Ring1.Height / 2);
        Canvas.SetLeft(P2Ring2, cx - P2Ring2.Width / 2); Canvas.SetTop(P2Ring2, cy - P2Ring2.Height / 2);
        Canvas.SetLeft(P2Poly0, cx - 50); Canvas.SetTop(P2Poly0, cy - 55);
        Canvas.SetLeft(P2Poly1, cx - 65); Canvas.SetTop(P2Poly1, cy - 65);
        Canvas.SetLeft(P2Flash0, cx - 160); Canvas.SetTop(P2Flash0, cy - 50);
        Canvas.SetLeft(P2Flash1, cx - 130); Canvas.SetTop(P2Flash1, cy + 70);
        Canvas.SetLeft(P2Word0, cx); Canvas.SetTop(P2Word0, cy - 90);
        Canvas.SetLeft(P2Word1, cx); Canvas.SetTop(P2Word1, cy - 20);
        Canvas.SetLeft(P2Word2, cx); Canvas.SetTop(P2Word2, cy + 50);
    }

    private void PositionScene3(double cx, double cy)
    {
        Canvas.SetLeft(GeoCoreHost, cx - GeoCoreHost.Width / 2);
        Canvas.SetTop(GeoCoreHost, cy - GeoCoreHost.Height / 2);
        Canvas.SetLeft(CoreGlowPulse, cx - CoreGlowPulse.Width / 2);
        Canvas.SetTop(CoreGlowPulse, cy - CoreGlowPulse.Height / 2);
        Canvas.SetLeft(Scene3Text, cx - 200);
        Canvas.SetTop(Scene3Text, cy + 165);
        Canvas.SetLeft(TechOverlay, 40);
        Canvas.SetTop(TechOverlay, 50);
    }

    private void PositionScene4(double cx, double cy)
    {
        Canvas.SetTop(Panel0, cy - 160);
        Canvas.SetTop(Panel1, cy - 20);
        Canvas.SetTop(Panel2, cy + 120);

        Canvas.SetLeft(S4Line1, cx - S4Line1.ActualWidth / 2.0);
        Canvas.SetTop(S4Line1, cy - 55);
        Canvas.SetLeft(S4Line2, cx - S4Line2.ActualWidth / 2.0);
        Canvas.SetTop(S4Line2, cy + 20);
    }

    private void PositionScene5(double cx, double cy)
    {
        Canvas.SetLeft(GeoCoreFinal, cx - GeoCoreFinal.Width / 2.0);
        Canvas.SetTop(GeoCoreFinal, cy - 80);
        Canvas.SetLeft(ShockRing, cx - ShockRing.Width / 2);
        Canvas.SetTop(ShockRing, cy - 80 - ShockRing.Height / 2);
        Canvas.SetLeft(RadialBurst, cx - 300);
        Canvas.SetTop(RadialBurst, cy - 200);
        Canvas.SetLeft(BrandTitle, cx - BrandTitle.Width / 2.0);
        Canvas.SetTop(BrandTitle, cy - 10);
        Canvas.SetLeft(BrandSub, cx - BrandSub.Width / 2.0);
        Canvas.SetTop(BrandSub, cy + 90);
        Canvas.SetLeft(BrandSig, cx - BrandSig.Width / 2.0);
        Canvas.SetTop(BrandSig, cy + 165);
    }

    private void BuildTimeline()
    {
        var sb = ShowreelTimeline.Build(
            // Scene 1
            AwakenLine, AwakenLineScale,
            new[] { HexFrag0, HexFrag1, HexFrag2, HexFrag3, HexFrag4, HexFrag5 },
            new[] { HexFrag0Tx, HexFrag1Tx, HexFrag2Tx, HexFrag3Tx, HexFrag4Tx, HexFrag5Tx },
            new[] { HexFrag0Rt, HexFrag1Rt, HexFrag2Rt, HexFrag3Rt, HexFrag4Rt, HexFrag5Rt },
            ScanBeam, ScanBeamTx,
            Scene1Text, Scene1TextTy,
            Scene1Root,

            // Scene 2
            Scene2Root,
            new[] { P2Ring0Rt, P2Ring1Rt, P2Ring2Rt },
            new[] { P2Poly0, P2Poly1 },
            new[] { P2Poly0Rt, P2Poly1Rt },
            new[] { P2Flash0, P2Flash1 },
            new[] { P2Word0, P2Word1, P2Word2 },
            new[] { P2Word0Tx, P2Word1Tx, P2Word2Tx },
            new[] { P2Word0Sc, P2Word1Sc, P2Word2Sc },

            // Scene 3
            Scene3Root,
            GeoCoreHost.CoreRotation,
            new[] { GeoCoreHost.Ring0Rotation, GeoCoreHost.Ring1Rotation, GeoCoreHost.Ring2Rotation },
            GeoCoreHost.HexInnerRotation,
            GeoCoreHost.ScaleRoot,
            CoreGlowPulse, CorePulse,
            TechOverlay, TechProgressTx,
            Scene3Text, Scene3TextTy,

            // Scene 4
            Scene4Root,
            new[] { Panel0, Panel1, Panel2 },
            new[] { Panel0Tx, Panel1Tx, Panel2Tx },
            S4Line1, S4Line1Tx, S4Line1Ty,
            S4Line2, S4Line2Tx, S4Line2Ty,

            // Scene 5
            Scene5Root,
            GeoCoreFinal.CoreRotation,
            new[] { GeoCoreFinal.Ring0Rotation, GeoCoreFinal.Ring1Rotation, GeoCoreFinal.Ring2Rotation },
            GeoCoreFinal.HexInnerRotation,
            GeoCoreFinal.ScaleRoot,
            ShockRing, ShockScale,
            RadialBurst, RadialBurstScale,
            BrandTitle, BrandTitleScale,
            BrandSub, BrandSubTy,
            BrandSig,

            // Global
            GlobalFade);

        // Per-character kinetic text for Scene 1: built once, added to Scene1Root,
        // then animated per char by the central storyboard (no per-frame layout).
        var s1Text = new KineticTypography
        {
            Text = "INNOVATION STARTS HERE",
            CharSize = 44,
            TextBrush = (Brush)FindResource("BrushWarmWhite"),
        };
        s1Text.Build();

        Scene1Root.Children.Add(s1Text);
        Canvas.SetLeft(s1Text, 0);
        Canvas.SetTop(s1Text, _center.Y + 120);
        s1Text.Width = ActualWidth;

        // The static headline is replaced by the kinetic one.
        Scene1Text.Visibility = Visibility.Collapsed;

        s1Text.RegisterReveal(sb, ShowreelTimeline.Scene1Start + 1.35, 0.022, 0.45, direction: 1);

        sb.Completed += (s, e) => _playing = false;

        _timeline = sb;
    }

    private void StartPlayback()
    {
        if (_timeline is null) return;

        _clock.Restart();
        _lastFrameSeconds = 0;
        _playing = true;
        Particles.SetMode(ParticleMode.EmitLine, _center.X, _center.Y, 200);
        _timeline.Begin(this, true);
        CompositionTarget.Rendering += OnRendering;
    }

    private void OnRendering(object? sender, EventArgs e)
    {
        if (!_playing) return;

        double t = _clock.Elapsed.TotalSeconds;

        // Measure FPS (rolling ~0.5 s window).
        double dt = t - _lastFrameSeconds;
        _lastFrameSeconds = t;
        _frameAccum += dt;
        _frameCount++;
        if (_frameAccum >= 0.5)
        {
            _fps = _frameCount / _frameAccum;
            _frameAccum = 0;
            _frameCount = 0;
            if (DebugOverlay.Visibility == Visibility.Visible)
                DebugOverlay.Text = $"FPS: {_fps:F1}   t: {t:F2}s";
        }

        // Drive particle layer from the master clock.
        Particles.Update(t);

        if (t >= ShowreelTimeline.TotalDuration.TotalSeconds)
        {
            StopPlayback();
        }
    }

    private void StopPlayback()
    {
        if (!_playing) return;
        _playing = false;
        CompositionTarget.Rendering -= OnRendering;
        Particles.SetMode(ParticleMode.Idle, 0, 0);
    }

    private void Restart()
    {
        StopPlayback();
        _timeline?.Remove(this);

        _clock.Restart();
        _lastFrameSeconds = 0;
        _playing = true;
        Particles.SetMode(ParticleMode.EmitLine, _center.X, _center.Y, 200);
        _timeline?.Begin(this, true);
        CompositionTarget.Rendering += OnRendering;
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Close();
        }
        else if (e.Key == Key.Space || e.Key == Key.Enter)
        {
            Restart();
        }
        else if (e.Key == Key.F11)
        {
            ToggleFullscreen();
        }
        else if (e.Key == Key.D)
        {
            DebugOverlay.Visibility = DebugOverlay.Visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
        }
    }

    private void ToggleFullscreen()
    {
        if (WindowStyle == WindowStyle.None)
        {
            WindowStyle = WindowStyle.SingleBorderWindow;
            WindowState = WindowState.Normal;
            ResizeMode = ResizeMode.CanResize;
        }
        else
        {
            WindowStyle = WindowStyle.None;
            WindowState = WindowState.Maximized;
        }
    }

    private void Window_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        // Detach rendering to prevent callbacks during teardown.
        CompositionTarget.Rendering -= OnRendering;
        _timeline?.Remove(this);
        _timeline = null;
        Particles.SetMode(ParticleMode.Idle, 0, 0);
    }
}
