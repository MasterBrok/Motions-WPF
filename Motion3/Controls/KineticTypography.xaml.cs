using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Motion3.Controls;

/// <summary>
/// Per-character kinetic text. Characters are created once at build time; the
/// central storyboard animates each char's Opacity + TranslateTransform.X via
/// direct element targets (deterministic, no name-scope lookups).
/// </summary>
public partial class KineticTypography : UserControl
{
    private readonly List<TextBlock> _chars = new();

    public KineticTypography()
    {
        InitializeComponent();
    }

    public string Text { get; set; } = string.Empty;
    public double CharSize { get; set; } = 72;
    public Brush? TextBrush { get; set; }

    public int CharCount => _chars.Count;

    /// <summary>Build character TextBlocks. Call once before playback starts.</summary>
    public void Build()
    {
        _chars.Clear();
        CharHost.Children.Clear();

        foreach (char ch in Text ?? string.Empty)
        {
            var tb = new TextBlock
            {
                Text = ch == ' ' ? "\u00A0" : ch.ToString(), // nbsp preserves spacing
                FontFamily = (FontFamily)FindResource("FontPrimary"),
                FontWeight = FontWeights.Bold,
                FontSize = CharSize,
                Foreground = TextBrush ?? (Brush)FindResource("BrushWarmWhite"),
                Opacity = 0,
                RenderTransform = new TranslateTransform(),
            };
            CharHost.Children.Add(tb);
            _chars.Add(tb);
        }
    }

    /// <summary>
    /// Adds staggered per-character reveal animations to the central storyboard.
    /// t0: absolute start in seconds; stagger: per-char delay; direction: -1 left / +1 right.
    /// </summary>
    public void RegisterReveal(Storyboard sb, double t0, double stagger, double charDuration, int direction = 1)
    {
        for (int i = 0; i < _chars.Count; i++)
        {
            var ch = _chars[i];
            double start = t0 + i * stagger;

            var op = new DoubleAnimation
            {
                From = 0,
                To = 1,
                BeginTime = TimeSpan.FromSeconds(start),
                Duration = TimeSpan.FromSeconds(charDuration),
                EasingFunction = (IEasingFunction)Application.Current.Resources["EaseOutCubic"],
                FillBehavior = FillBehavior.HoldEnd,
            };
            Storyboard.SetTarget(op, ch);
            Storyboard.SetTargetProperty(op, new PropertyPath("Opacity"));
            sb.Children.Add(op);

            var tx = new DoubleAnimation
            {
                From = direction * 30,
                To = 0,
                BeginTime = TimeSpan.FromSeconds(start),
                Duration = TimeSpan.FromSeconds(charDuration * 1.3),
                EasingFunction = (IEasingFunction)Application.Current.Resources["EaseOutQuint"],
                FillBehavior = FillBehavior.HoldEnd,
            };
            Storyboard.SetTarget(tx, ch);
            Storyboard.SetTargetProperty(tx, new PropertyPath("RenderTransform.X"));
            sb.Children.Add(tx);
        }
    }

    /// <summary>Hide all chars (used before restart).</summary>
    public void ResetState()
    {
        foreach (var ch in _chars)
        {
            ch.BeginAnimation(TextBlock.OpacityProperty, null);
            ch.Opacity = 0;
            ((TranslateTransform)ch.RenderTransform).X = 0;
        }
    }
}
