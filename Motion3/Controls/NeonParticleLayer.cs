using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace Motion3.Controls;

public enum ParticleMode { Idle, EmitLine, Orbit, Burst }

/// <summary>
/// Bounded, allocation-free particle layer drawn through a single DrawingVisual.
/// Position updates derive from absolute elapsed time (deterministic, frame-rate independent).
/// </summary>
public sealed class NeonParticleLayer : FrameworkElement
{
    public const int HardCapacity = 512;

    private readonly Particle[] _particles = new Particle[HardCapacity];
    private readonly DrawingVisual _dv = new();

    // Frozen pen cache (3 accent colors)
    private static readonly Pen[] ParticlePens = CreatePens();

    // Alpha-quantized brush cache: 3 colors x 33 alpha steps, preallocated once.
    private static readonly Brush[,] DotBrushes = CreateDotBrushes();

    private ParticleMode _mode = ParticleMode.Idle;
    private double _cx, _cy;
    private double _sceneRadius = 200;
    private int _maxParticles = 220;
    private int _aliveCount;

    public int MaxParticles
    {
        get => _maxParticles;
        set => _maxParticles = Math.Clamp(value, 1, HardCapacity);
    }

    public NeonParticleLayer()
    {
        for (int i = 0; i < HardCapacity; i++) _particles[i] = new Particle();
        AddVisualChild(_dv);
        AddLogicalChild(_dv);
    }

    private static Pen[] CreatePens()
    {
        var cyan = new Pen(new SolidColorBrush(Color.FromRgb(0, 229, 255)), 1.0);
        var magenta = new Pen(new SolidColorBrush(Color.FromRgb(255, 0, 122)), 1.0);
        var purple = new Pen(new SolidColorBrush(Color.FromRgb(168, 85, 247)), 1.0);
        cyan.Freeze(); magenta.Freeze(); purple.Freeze();
        return new[] { cyan, magenta, purple };
    }

    private static Brush[,] CreateDotBrushes()
    {
        var baseColors = new[]
        {
            Color.FromRgb(0, 229, 255),    // cyan
            Color.FromRgb(255, 0, 122),    // magenta
            Color.FromRgb(240, 244, 255),  // warm white
        };

        var brushes = new Brush[3, 33];
        for (int c = 0; c < 3; c++)
        {
            for (int a = 0; a <= 32; a++)
            {
                var col = baseColors[c];
                col.A = (byte)(a * 255 / 32);
                var b = new SolidColorBrush(col);
                b.Freeze();
                brushes[c, a] = b;
            }
        }
        return brushes;
    }

    /// <summary>Configure layer for a scene. cx/cy are element-local coordinates.</summary>
    public void SetMode(ParticleMode mode, double cx, double cy, double sceneRadius = 200)
    {
        _mode = mode;
        _cx = cx;
        _cy = cy;
        _sceneRadius = sceneRadius;
    }

    /// <summary>Advance simulation to absolute timeline time t (seconds) and redraw.</summary>
    public void Update(double t)
    {
        if (_mode == ParticleMode.Idle || t <= 0)
        {
            if (_aliveCount != 0)
            {
                _aliveCount = 0;
                Redraw();
            }
            return;
        }

        switch (_mode)
        {
            case ParticleMode.EmitLine: SimulateEmitLine(t); break;
            case ParticleMode.Orbit: SimulateOrbit(t); break;
            case ParticleMode.Burst: SimulateBurst(t); break;
        }

        Redraw();
    }

    // Scene 1: particles stream outward from an expanding horizontal line.
    private void SimulateEmitLine(double t)
    {
        double halfWidth = Math.Min(ActualWidth * 0.42, 560);
        if (halfWidth <= 0) halfWidth = 480;
        _aliveCount = 0;

        for (int i = 0; i < _maxParticles; i++)
        {
            double seed = i * 0.6180339887; // golden-ratio scramble, deterministic
            // Each particle loops with an individual period; fractional position = deterministic phase.
            double period = 1.6 + (seed % 1.0) * 1.4;
            double phase = (t / period + seed * 0.731) % 1.0;
            double alpha = Math.Sin(phase * Math.PI); // fade in/out over life

            double side = (seed * 3.0) % 1.0 < 0.5 ? -1 : 1;
            double lateralSpeed = 120 + (seed % 1.0) * 340;
            double x = _cx + side * phase * lateralSpeed;
            double y = _cy + Math.Sin(seed * 12.9 + t * (0.6 + (seed % 1.0) * 0.8)) * (14 + (seed % 1.0) * 26);

            if (x < -40 || x > ActualWidth + 40) continue;

            _particles[_aliveCount].X = x;
            _particles[_aliveCount].Y = y;
            _particles[_aliveCount].Alpha = alpha * (0.5 + 0.5 * (seed % 1.0));
            _particles[_aliveCount].ColorIndex = (int)(seed * 7.0) % 3;
            _aliveCount++;
        }
    }

    // Scene 3: particles orbit the geometric core on precessing ellipses.
    private void SimulateOrbit(double t)
    {
        _aliveCount = 0;

        for (int i = 0; i < _maxParticles; i++)
        {
            double seed = i * 0.6180339887;
            double angle = seed * Math.Tau + t * (0.35 + (seed % 1.0) * 0.45) * (i % 2 == 0 ? 1 : -1);
            double rBase = _sceneRadius * (0.62 + 0.5 * (seed % 1.0));
            double wobble = Math.Sin(t * 0.9 + seed * 5.1) * 10;
            double x = _cx + Math.Cos(angle) * (rBase + wobble);
            double y = _cy + Math.Sin(angle) * (rBase + wobble) * 0.42; // elliptical, pseudo-3D tilt
            double alpha = 0.35 + 0.55 * (0.5 + 0.5 * Math.Sin(seed * 9.4 + t * 1.7));

            _particles[_aliveCount].X = x;
            _particles[_aliveCount].Y = y;
            _particles[_aliveCount].Alpha = alpha;
            _particles[_aliveCount].ColorIndex = (int)(seed * 11.0) % 3;
            _aliveCount++;
        }
    }

    // Scene 5: radial burst outward from the contracted core point.
    private void SimulateBurst(double t)
    {
        const double burstStart = 11.0;
        const double burstDuration = 2.6;
        double bt = t - burstStart;
        _aliveCount = 0;

        if (bt <= 0) return;
        if (bt > burstDuration) bt = burstDuration;

        double progress = bt / burstDuration;

        for (int i = 0; i < _maxParticles; i++)
        {
            double seed = i * 0.6180339887;
            double angle = seed * Math.Tau;
            double dist = EaseOutQuart(progress) * (140 + (seed % 1.0) * 420);
            double alpha = (1.0 - progress) * (0.35 + 0.65 * (seed % 1.0));

            _particles[_aliveCount].X = _cx + Math.Cos(angle) * dist;
            _particles[_aliveCount].Y = _cy + Math.Sin(angle) * dist * 0.55;
            _particles[_aliveCount].Alpha = alpha;
            _particles[_aliveCount].ColorIndex = (int)(seed * 13.0) % 3;
            _aliveCount++;
        }
    }

    private static double EaseOutQuart(double x) => 1.0 - Math.Pow(1.0 - x, 4);

    private void Redraw()
    {
        double w = ActualWidth > 0 ? ActualWidth : 1280;
        double h = ActualHeight > 0 ? ActualHeight : 720;

        using var ctx = _dv.RenderOpen();

        for (int i = 0; i < _aliveCount; i++)
        {
            ref readonly Particle p = ref _particles[i];
            int alphaIdx = (int)(p.Alpha * 32);
            if (alphaIdx <= 0) continue;
            if (alphaIdx > 32) alphaIdx = 32;

            var brush = DotBrushes[p.ColorIndex, alphaIdx];
            ctx.DrawEllipse(brush, null, new Point(p.X, p.Y), 1.4, 1.4);

            // soft halo for brighter particles (cheap: one extra ellipse, no bitmap effects)
            if (alphaIdx >= 22)
            {
                var halo = DotBrushes[p.ColorIndex, alphaIdx / 3];
                ctx.DrawEllipse(halo, null, new Point(p.X, p.Y), 3.6, 3.6);
            }
        }
    }

    protected override int VisualChildrenCount => 1;
    protected override Visual GetVisualChild(int index) => _dv;

    private struct Particle
    {
        public double X, Y, Alpha;
        public int ColorIndex;
    }
}
