using System.Windows.Media.Media3D;
using Chronos.Models;

namespace Chronos.Animation;

public class MechanicalAnimator
{
    private readonly TimelineController _timeline;
    private readonly MechanicalHeart _heart;
    private QualityLevel _quality = QualityLevel.High;

    public MechanicalAnimator(TimelineController timeline, MechanicalHeart heart)
    {
        _timeline = timeline;
        _heart = heart;
    }

    public void SetQuality(QualityLevel level)
    {
        _quality = level;
    }

    public void Update()
    {
        double t = _timeline.CurrentTime.TotalSeconds;
        int scene = _timeline.CurrentScene;

        switch (scene)
        {
            case 0: UpdateAwakening(t); break;
            case 1: UpdateRevelation(t); break;
            case 2: UpdateDisassembly(t); break;
            case 3: UpdateEnergySurge(t); break;
            case 4: UpdateTitle(t); break;
            case 5: UpdateFinal(t); break;
        }
    }

    private void UpdateAwakening(double t)
    {
        double phase = TimelineController.EaseIn(Math.Min(t / 5.0, 1));

        _heart.RingsAngle.Angle = t * 20;
        _heart.RingsOffset.OffsetZ = (1 - phase) * 10;
        _heart.RingsOffset.OffsetX = 0;
        _heart.RingsOffset.OffsetY = 0;

        _heart.GearsAngle.Angle = -t * 30;
        _heart.PlatesOffset.OffsetZ = 0;

        double s = 0.3 + phase * 0.7 + 0.05 * Math.Sin(t * 4);
        SetCoreScale(s);
    }

    private void UpdateRevelation(double t)
    {
        _heart.RingsAngle.Angle = t * 20;
        _heart.RingsOffset.OffsetZ = 0;
        _heart.GearsAngle.Angle = t * 40;
        _heart.PlatesAngle.Angle = t * 10;
        _heart.PlatesOffset.OffsetX = Math.Sin(t) * 2;
        _heart.PlatesOffset.OffsetY = Math.Cos(t * 0.7) * 2;

        SetCoreScale(1.0 + 0.05 * Math.Sin(t * 4));
    }

    private void UpdateDisassembly(double t)
    {
        double phase = TimelineController.EaseInOut(Math.Min((t - 10) / 6.0, 1));

        _heart.RingsAngle.Angle = t * 30;
        _heart.RingsOffset.OffsetY = phase * 2.5;
        _heart.RingsOffset.OffsetX = phase * 1.5;

        _heart.GearsAngle.Angle = -t * 45;
        _heart.PlatesAngle.Angle = t * 40;
        _heart.PlatesOffset.OffsetX = Math.Sin(t * 1.5) * 4;
        _heart.PlatesOffset.OffsetY = Math.Cos(t * 1.1) * 3;
        _heart.PlatesOffset.OffsetZ = phase * 3;

        SetCoreScale(1.0 + 0.15 * Math.Sin(t * 4));
    }

    private void UpdateEnergySurge(double t)
    {
        double phase = TimelineController.EaseOut(Math.Min((t - 16) / 6.0, 1));

        _heart.RingsAngle.Angle = t * (60 + phase * 80);
        _heart.RingsOffset.OffsetX = (1 - phase) * 1.5;
        _heart.RingsOffset.OffsetY = (1 - phase) * 2.0;
        _heart.RingsOffset.OffsetZ = (1 - phase) * 1.5;

        _heart.GearsAngle.Angle = -t * (90 + phase * 60);
        _heart.PlatesAngle.Angle = t * 20;
        _heart.PlatesOffset.OffsetX = (1 - phase) * 4;
        _heart.PlatesOffset.OffsetY = (1 - phase) * 3;
        _heart.PlatesOffset.OffsetZ = (1 - phase) * 3;

        SetCoreScale(1.0 + 0.2 * Math.Sin(t * 8) * (1 - phase) + phase * 0.1);
    }

    private void UpdateTitle(double t)
    {
        _heart.RingsAngle.Angle = 60 + t * 5;
        _heart.RingsOffset.OffsetX = 0;
        _heart.RingsOffset.OffsetY = 0;
        _heart.RingsOffset.OffsetZ = 0;

        _heart.GearsAngle.Angle = 30 + t * 10;
        _heart.PlatesAngle.Angle = 0;
        _heart.PlatesOffset.OffsetX = 0;
        _heart.PlatesOffset.OffsetY = 0;
        _heart.PlatesOffset.OffsetZ = 0;

        SetCoreScale(0.9 + 0.05 * Math.Sin(t * 3));
    }

    private void UpdateFinal(double t)
    {
        _heart.RingsAngle.Angle = 60 + t * 8;
        _heart.GearsAngle.Angle = -t * 20;

        SetCoreScale(1.0 + 0.04 * Math.Sin(t * 5));
    }

    private void SetCoreScale(double s)
    {
        _heart.CoreScaleTransform.ScaleX = s;
        _heart.CoreScaleTransform.ScaleY = s;
        _heart.CoreScaleTransform.ScaleZ = s;
    }
}
