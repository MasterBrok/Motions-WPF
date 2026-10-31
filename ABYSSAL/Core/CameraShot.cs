using System.Windows.Media.Media3D;

namespace ABYSSAL;

public class CameraShot
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public Point3D StartPosition { get; set; }
    public Point3D EndPosition { get; set; }
    public Point3D LookTarget { get; set; }
    public TransitionType TransitionType { get; set; }

    public double GetProgress(TimeSpan time)
    {
        if (time < StartTime) return 0;
        if (time > EndTime) return 1;

        var duration = (EndTime - StartTime).TotalSeconds;
        if (duration <= 0) return 1;

        return (time - StartTime).TotalSeconds / duration;
    }

    public Point3D GetPosition(TimeSpan time)
    {
        var t = GetProgress(time);
        return new Point3D(
            Lerp(StartPosition.X, EndPosition.X, t),
            Lerp(StartPosition.Y, EndPosition.Y, t),
            Lerp(StartPosition.Z, EndPosition.Z, t)
        );
    }

    private static double Lerp(double a, double b, double t)
    {
        return a + (b - a) * t;
    }
}
