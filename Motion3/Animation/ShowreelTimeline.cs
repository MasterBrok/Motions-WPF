using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Motion3.Animation;

/// <summary>
/// Central, deterministic 15-second timeline. Builds the complete storyboard in
/// one pass, targeting elements directly via Storyboard.SetTarget/SetTargetProperty
/// (no name-scope lookups, no cross-UserControl naming issues).
/// </summary>
public static class ShowreelTimeline
{
    public static readonly TimeSpan TotalDuration = TimeSpan.FromSeconds(15);
    public const double Scene1Start = 0.30;
    public const double Scene2Start = 2.50;
    public const double Scene3Start = 5.00;
    public const double Scene4Start = 8.00;
    public const double Scene5Start = 11.00;

    // =====================================================================
    // Public entry: call from MainWindow with all named elements.
    // =====================================================================
    public static Storyboard Build(
        // Scene 1 elements
        FrameworkElement awakenLine,
        ScaleTransform awakenLineScale,
        FrameworkElement[] hexFrags, TranslateTransform[] hexFragTxs, RotateTransform[] hexFragRts,
        FrameworkElement scanBeam, TranslateTransform scanBeamTx,
        FrameworkElement scene1Text, TranslateTransform scene1TextTy,
        FrameworkElement scene1Root,

        // Scene 2 elements
        FrameworkElement scene2Root,
        RotateTransform[] p2RingRts,         // Ring0, Ring1, Ring2
        FrameworkElement[] p2Polys, RotateTransform[] p2PolyRts,
        FrameworkElement[] p2Flashes,
        FrameworkElement[] p2Words, TranslateTransform[] p2WordTxs, ScaleTransform[] p2WordScs,

        // Scene 3 elements
        FrameworkElement scene3Root,
        RotateTransform hostCoreRotation,
        RotateTransform[] hostRingRts,       // Ring0, Ring1, Ring2
        RotateTransform hostHexInnerRotation,
        ScaleTransform hostScaleRoot,
        FrameworkElement coreGlowPulse, ScaleTransform corePulse,
        FrameworkElement techOverlay, TranslateTransform techProgressTx,
        FrameworkElement scene3Text, TranslateTransform scene3TextTy,

        // Scene 4 elements
        FrameworkElement scene4Root,
        FrameworkElement[] panels, TranslateTransform[] panelTxs,
        FrameworkElement s4Line1, TranslateTransform s4Line1Tx, TranslateTransform s4Line1Ty,
        FrameworkElement s4Line2, TranslateTransform s4Line2Tx, TranslateTransform s4Line2Ty,

        // Scene 5 elements
        FrameworkElement scene5Root,
        RotateTransform finalCoreRotation,
        RotateTransform[] finalRingRts,
        RotateTransform finalHexInnerRotation,
        ScaleTransform finalScaleRoot,
        FrameworkElement shockRing, ScaleTransform shockScale,
        FrameworkElement radialBurst, ScaleTransform radialBurstScale,
        FrameworkElement brandTitle, ScaleTransform brandTitleScale,
        FrameworkElement brandSub, TranslateTransform brandSubTy,
        FrameworkElement brandSig,

        // Global
        FrameworkElement globalFade)
    {
        var sb = new Storyboard { Duration = TotalDuration, FillBehavior = FillBehavior.HoldEnd };

        // ---- Scene 1 --------------------------------------------------------
        Add(sb, 0.0, 1.0, Scene1Start, 0.8, "EaseOutQuart", awakenLineScale, ScaleTransform.ScaleXProperty);
        Add(sb, 0.0, 1.0, Scene1Start, 0.8, "EaseOutQuart", awakenLineScale, ScaleTransform.ScaleYProperty);
        Add(sb, 0.0, 1.0, Scene1Start + 0.1, 0.5, "EaseOutCubic", awakenLine, UIElement.OpacityProperty);
        Add(sb, 1.0, 0.0, Scene1Start + 1.6, 0.5, "EaseInCubic", awakenLine, UIElement.OpacityProperty);

        for (int i = 0; i < hexFrags.Length; i++)
        {
            double t0 = Scene1Start + 0.55 + i * 0.08;
            Add(sb, 0.0, 1.0, t0, 0.6, "EaseOutCubic", hexFrags[i], UIElement.OpacityProperty);
            Add(sb, i % 2 == 0 ? -260 : 260, 0, t0, 0.7, "EaseOutQuint", hexFragTxs[i], TranslateTransform.XProperty);
            Add(sb, i < 3 ? -160 : 160, 0, t0, 0.7, "EaseOutQuint", hexFragTxs[i], TranslateTransform.YProperty);
            Add(sb, i % 2 == 0 ? -90 : 90, 0, t0, 0.9, "EaseOutQuart", hexFragRts[i], RotateTransform.AngleProperty);
        }

        Add(sb, -300, 1400, Scene1Start + 0.9, 0.9, "EaseInOutCubic", scanBeamTx, TranslateTransform.XProperty);
        Add(sb, 0.0, 0.65, Scene1Start + 0.85, 0.15, "EaseOutCubic", scanBeam, UIElement.OpacityProperty);
        Add(sb, 0.65, 0.0, Scene1Start + 1.5, 0.4, "EaseInCubic", scanBeam, UIElement.OpacityProperty);

        // Headline container (static, replaced by kinetic chars)
        Add(sb, 0.0, 1.0, Scene1Start + 1.25, 0.7, "EaseOutCubic", scene1Text, UIElement.OpacityProperty);
        Add(sb, 24.0, 0.0, Scene1Start + 1.25, 0.9, "EaseOutQuint", scene1TextTy, TranslateTransform.YProperty);
        Add(sb, 1.0, 0.0, Scene2Start - 0.35, 0.35, "EaseInCubic", scene1Root, UIElement.OpacityProperty);

        // ---- Scene 2 --------------------------------------------------------
        Add(sb, 0.0, 1.0, Scene2Start - 0.1, 0.35, "EaseOutCubic", scene2Root, UIElement.OpacityProperty);

        Add(sb, 0.0, 360.0, Scene2Start, 2.5, "EaseInOutSine", p2RingRts[0], RotateTransform.AngleProperty);
        Add(sb, 360.0, 0.0, Scene2Start, 2.5, "EaseInOutSine", p2RingRts[1], RotateTransform.AngleProperty);
        Add(sb, 0.0, 220.0, Scene2Start, 2.5, "EaseInOutSine", p2RingRts[2], RotateTransform.AngleProperty);

        Add(sb, 0.0, 1.0, Scene2Start + 0.1, 0.6, "EaseOutBack", p2Polys[0], UIElement.OpacityProperty);
        Add(sb, 0.0, 1.0, Scene2Start + 0.35, 0.6, "EaseOutBack", p2Polys[1], UIElement.OpacityProperty);
        Add(sb, 0.0, 220.0, Scene2Start + 0.1, 2.4, "EaseInOutSine", p2PolyRts[0], RotateTransform.AngleProperty);
        Add(sb, 220.0, 0.0, Scene2Start + 0.35, 2.2, "EaseInOutSine", p2PolyRts[1], RotateTransform.AngleProperty);

        Add(sb, 0.0, 0.9, Scene2Start + 0.55, 0.12, "EaseOutCubic", p2Flashes[0], UIElement.OpacityProperty);
        Add(sb, 0.9, 0.0, Scene2Start + 0.75, 0.3, "EaseInCubic", p2Flashes[0], UIElement.OpacityProperty);
        Add(sb, 0.0, 0.8, Scene2Start + 1.15, 0.12, "EaseOutCubic", p2Flashes[1], UIElement.OpacityProperty);
        Add(sb, 0.8, 0.0, Scene2Start + 1.35, 0.3, "EaseInCubic", p2Flashes[1], UIElement.OpacityProperty);

        for (int i = 0; i < p2Words.Length; i++)
        {
            double t0 = Scene2Start + 0.45 + i * 0.45;
            Add(sb, 0.0, 1.0, t0, 0.4, "EaseOutCubic", p2Words[i], UIElement.OpacityProperty);
            Add(sb, 0.65, 1.0, t0, 0.55, "EaseOutBack", p2WordScs[i], ScaleTransform.ScaleXProperty);
            Add(sb, 0.65, 1.0, t0, 0.55, "EaseOutBack", p2WordScs[i], ScaleTransform.ScaleYProperty);
            Add(sb, 18.0, 0.0, t0, 0.5, "EaseOutQuint", p2WordTxs[i], TranslateTransform.YProperty);
        }

        Add(sb, 1.0, 0.0, Scene3Start - 0.35, 0.35, "EaseInCubic", scene2Root, UIElement.OpacityProperty);

        // ---- Scene 3 --------------------------------------------------------
        Add(sb, 0.0, 1.0, Scene3Start - 0.1, 0.4, "EaseOutCubic", scene3Root, UIElement.OpacityProperty);

        Add(sb, 0.25, 1.0, Scene3Start, 0.9, "EaseOutBack", hostScaleRoot, ScaleTransform.ScaleXProperty);
        Add(sb, 0.25, 1.0, Scene3Start, 0.9, "EaseOutBack", hostScaleRoot, ScaleTransform.ScaleYProperty);
        Add(sb, 0.0, 1.0, Scene3Start, 0.5, "EaseOutCubic", scene3Root, UIElement.OpacityProperty);

        // Core rotation + counter-rotations
        Add(sb, 0.0, 720.0, Scene3Start, 3.0, "EaseInOutSine", hostCoreRotation, RotateTransform.AngleProperty);
        Add(sb, 0.0, 630.0, Scene3Start, 3.0, "EaseInOutSine", hostRingRts[0], RotateTransform.AngleProperty);
        Add(sb, 0.0, -540.0, Scene3Start, 3.0, "EaseInOutSine", hostRingRts[1], RotateTransform.AngleProperty);
        Add(sb, 720.0, 0.0, Scene3Start, 3.0, "EaseInOutSine", hostRingRts[2], RotateTransform.AngleProperty);
        Add(sb, 0.0, -270.0, Scene3Start, 3.0, "EaseInOutSine", hostHexInnerRotation, RotateTransform.AngleProperty);

        // Energy pulses (CorePulse = ScaleTransform on the glow ellipse)
        for (int p = 0; p < 2; p++)
        {
            double t0 = Scene3Start + 0.7 + p * 1.1;
            Add(sb, 1.0, 1.07, t0, 0.22, "EaseOutCubic", corePulse, ScaleTransform.ScaleXProperty);
            Add(sb, 1.0, 1.07, t0, 0.22, "EaseOutCubic", corePulse, ScaleTransform.ScaleYProperty);
            Add(sb, 1.07, 1.0, t0 + 0.22, 0.35, "EaseInOutCubic", corePulse, ScaleTransform.ScaleXProperty);
            Add(sb, 1.07, 1.0, t0 + 0.22, 0.35, "EaseInOutCubic", corePulse, ScaleTransform.ScaleYProperty);
            Add(sb, 0.35, 0.85, t0, 0.22, "EaseOutCubic", coreGlowPulse, UIElement.OpacityProperty);
            Add(sb, 0.85, 0.35, t0 + 0.22, 0.5, "EaseInOutCubic", coreGlowPulse, UIElement.OpacityProperty);
        }

        Add(sb, 0.0, 1.0, Scene3Start + 0.4, 0.5, "EaseOutCubic", techOverlay, UIElement.OpacityProperty);
        Add(sb, -70.0, 70.0, Scene3Start + 0.5, 2.2, "EaseInOutCubic", techProgressTx, TranslateTransform.XProperty);
        Add(sb, 0.0, 1.0, Scene3Start + 0.9, 0.6, "EaseOutCubic", scene3Text, UIElement.OpacityProperty);
        Add(sb, 20.0, 0.0, Scene3Start + 0.9, 0.7, "EaseOutQuint", scene3TextTy, TranslateTransform.YProperty);
        Add(sb, 1.0, 0.0, Scene4Start - 0.35, 0.35, "EaseInCubic", scene3Root, UIElement.OpacityProperty);

        // ---- Scene 4 --------------------------------------------------------
        Add(sb, 0.0, 1.0, Scene4Start - 0.15, 0.3, "EaseOutCubic", scene4Root, UIElement.OpacityProperty);

        Add(sb, -1600, 0, Scene4Start, 0.5, "EaseOutQuint", panelTxs[0], TranslateTransform.XProperty);
        Add(sb, 0.0, 1.0, Scene4Start, 0.25, "EaseOutCubic", panels[0], UIElement.OpacityProperty);
        Add(sb, 1600, 0, Scene4Start + 0.12, 0.45, "EaseOutQuint", panelTxs[1], TranslateTransform.XProperty);
        Add(sb, 0.0, 1.0, Scene4Start + 0.12, 0.25, "EaseOutCubic", panels[1], UIElement.OpacityProperty);
        Add(sb, -1800, 0, Scene4Start + 0.24, 0.5, "EaseOutQuint", panelTxs[2], TranslateTransform.XProperty);
        Add(sb, 0.0, 1.0, Scene4Start + 0.24, 0.25, "EaseOutCubic", panels[2], UIElement.OpacityProperty);

        Add(sb, 0.0, -1900, Scene4Start + 0.9, 0.55, "EaseInQuart", panelTxs[0], TranslateTransform.XProperty);
        Add(sb, 1.0, 0.0, Scene4Start + 1.0, 0.5, "EaseInCubic", panels[0], UIElement.OpacityProperty);
        Add(sb, 0.0, 1900, Scene4Start + 1.0, 0.5, "EaseInQuart", panelTxs[1], TranslateTransform.XProperty);
        Add(sb, 1.0, 0.0, Scene4Start + 1.1, 0.5, "EaseInCubic", panels[1], UIElement.OpacityProperty);
        Add(sb, 0.0, -1900, Scene4Start + 1.1, 0.5, "EaseInQuart", panelTxs[2], TranslateTransform.XProperty);
        Add(sb, 1.0, 0.0, Scene4Start + 1.2, 0.5, "EaseInCubic", panels[2], UIElement.OpacityProperty);

        Add(sb, 0.0, 1.0, Scene4Start + 0.9, 0.55, "EaseOutCubic", s4Line1, UIElement.OpacityProperty);
        Add(sb, -160, 0, Scene4Start + 0.9, 0.8, "EaseOutQuint", s4Line1Tx, TranslateTransform.XProperty);
        Add(sb, 0.0, 1.0, Scene4Start + 1.35, 0.55, "EaseOutCubic", s4Line2, UIElement.OpacityProperty);
        Add(sb, 220, 0, Scene4Start + 1.35, 0.8, "EaseOutQuint", s4Line2Tx, TranslateTransform.XProperty);
        Add(sb, 0, -14, Scene4Start + 2.0, 0.6, "EaseInOutCubic", s4Line1Ty, TranslateTransform.YProperty);
        Add(sb, 0, 10, Scene4Start + 2.0, 0.7, "EaseInOutCubic", s4Line2Ty, TranslateTransform.YProperty);
        Add(sb, 1.0, 0.0, Scene5Start - 0.3, 0.3, "EaseInCubic", scene4Root, UIElement.OpacityProperty);

        // ---- Scene 5 --------------------------------------------------------
        Add(sb, 0.0, 1.0, Scene5Start - 0.05, 0.2, "EaseOutCubic", scene5Root, UIElement.OpacityProperty);

        // Core contract
        Add(sb, 1.0, 0.0, Scene5Start, 0.7, "EaseInQuart", finalScaleRoot, ScaleTransform.ScaleXProperty);
        Add(sb, 1.0, 0.0, Scene5Start, 0.7, "EaseInQuart", finalScaleRoot, ScaleTransform.ScaleYProperty);
        Add(sb, 1.0, 0.0, Scene5Start + 0.45, 0.4, "EaseInCubic", scene5Root, UIElement.OpacityProperty);

        // Shockwave ring
        Add(sb, 0.2, 1.0, Scene5Start + 0.55, 1.1, "EaseOutQuart", shockScale, ScaleTransform.ScaleXProperty);
        Add(sb, 0.2, 1.0, Scene5Start + 0.55, 1.1, "EaseOutQuart", shockScale, ScaleTransform.ScaleYProperty);
        Add(sb, 0.0, 0.95, Scene5Start + 0.55, 0.12, "EaseOutCubic", shockRing, UIElement.OpacityProperty);
        Add(sb, 0.95, 0.0, Scene5Start + 0.75, 0.9, "EaseInCubic", shockRing, UIElement.OpacityProperty);
        Add(sb, 1.6, 0.0, Scene5Start + 0.6, 0.9, "EaseOutCubic", shockRing, Shape.StrokeThicknessProperty);

        // Radial burst
        Add(sb, 0.0, 0.9, Scene5Start + 0.6, 0.1, "EaseOutCubic", radialBurst, UIElement.OpacityProperty);
        Add(sb, 0.9, 0.0, Scene5Start + 0.8, 0.7, "EaseInCubic", radialBurst, UIElement.OpacityProperty);
        Add(sb, 0.4, 1.0, Scene5Start + 0.6, 0.9, "EaseOutQuart", radialBurstScale, ScaleTransform.ScaleXProperty);
        Add(sb, 0.4, 1.0, Scene5Start + 0.6, 0.9, "EaseOutQuart", radialBurstScale, ScaleTransform.ScaleYProperty);

        // Brand reveal
        Add(sb, 0.0, 1.0, Scene5Start + 0.85, 0.8, "EaseOutCubic", brandTitle, UIElement.OpacityProperty);
        Add(sb, 0.86, 1.0, Scene5Start + 0.8, 1.0, "EaseOutQuint", brandTitleScale, ScaleTransform.ScaleXProperty);
        Add(sb, 0.86, 1.0, Scene5Start + 0.8, 1.0, "EaseOutQuint", brandTitleScale, ScaleTransform.ScaleYProperty);
        Add(sb, 0.0, 1.0, Scene5Start + 1.6, 0.7, "EaseOutCubic", brandSub, UIElement.OpacityProperty);
        Add(sb, 18.0, 0.0, Scene5Start + 1.6, 0.8, "EaseOutQuint", brandSubTy, TranslateTransform.YProperty);
        Add(sb, 0.0, 1.0, Scene5Start + 2.3, 0.6, "EaseOutCubic", brandSig, UIElement.OpacityProperty);

        // ---- Global fade to black ------------------------------------------
        const double fadeStart = 14.2;
        Add(sb, 0.0, 1.0, fadeStart, 15.0 - fadeStart, "EaseInOutCubic", globalFade, UIElement.OpacityProperty);

        return sb;
    }

    // =====================================================================
    // Low-level: add a DoubleAnimation with a direct target and property path.
    // =====================================================================
    private static void Add(Storyboard sb, double from, double to, double begin,
        double dur, string easeKey, DependencyObject target, DependencyProperty prop)
    {
        var anim = new DoubleAnimation
        {
            From = from,
            To = to,
            BeginTime = TimeSpan.FromSeconds(begin),
            Duration = TimeSpan.FromSeconds(dur),
            EasingFunction = (IEasingFunction)Application.Current.Resources[easeKey],
            FillBehavior = FillBehavior.HoldEnd,
        };
        Storyboard.SetTarget(anim, target);
        Storyboard.SetTargetProperty(anim, new PropertyPath(prop));
        sb.Children.Add(anim);
    }
}