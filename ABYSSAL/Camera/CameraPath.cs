using System.Windows.Media.Media3D;

namespace ABYSSAL.Camera;

public class CameraPath
{
    private readonly List<PathKeyframe> _keyframes = new();

    public class PathKeyframe
    {
        public TimeSpan Time { get; set; }
        public Point3D Position { get; set; }
        public Point3D LookTarget { get; set; }
        public double FieldOfView { get; set; } = 60;
        public double Banking { get; set; } = 0;
    }

    public void AddKeyframe(TimeSpan time, Point3D position, Point3D lookTarget)
    {
        _keyframes.Add(new PathKeyframe
        {
            Time = time,
            Position = position,
            LookTarget = lookTarget
        });
        _keyframes.Sort((a, b) => a.Time.CompareTo(b.Time));
    }

    public (Point3D Position, Point3D LookTarget) Evaluate(TimeSpan time)
    {
        if (_keyframes.Count == 0)
        {
            return (new Point3D(0, 50, 0), new Point3D(0, 0, 0));
        }

        if (_keyframes.Count == 1 || time <= _keyframes[0].Time)
        {
            return (_keyframes[0].Position, _keyframes[0].LookTarget);
        }

        if (time >= _keyframes[^1].Time)
        {
            return (_keyframes[^1].Position, _keyframes[^1].LookTarget);
        }

        PathKeyframe? prev = null;
        PathKeyframe? next = null;

        for (int i = 0; i < _keyframes.Count - 1; i++)
        {
            if (time >= _keyframes[i].Time && time <= _keyframes[i + 1].Time)
            {
                prev = _keyframes[i];
                next = _keyframes[i + 1];
                break;
            }
        }

        if (prev == null || next == null)
        {
            return (_keyframes[0].Position, _keyframes[0].LookTarget);
        }

        var duration = (next.Time - prev.Time).TotalSeconds;
        var elapsed = (time - prev.Time).TotalSeconds;
        var t = duration > 0 ? elapsed / duration : 0;
        var easedT = CatmullRomEase(t);

        var position = CatmullRomInterpolate(prev.Position, next.Position, t);
        var lookTarget = CatmullRomInterpolate(prev.LookTarget, next.LookTarget, t);

        return (position, lookTarget);
    }

    private static Point3D CatmullRomInterpolate(Point3D p0, Point3D p1, double t)
    {
        var t2 = t * t;
        var t3 = t2 * t;

        var x = p1.X;
        var y = p1.Y;
        var z = p1.Z;

        return new Point3D(x, y, z);
    }

    private static double CatmullRomEase(double t)
    {
        return t * t * (3 - 2 * t);
    }

    public void Clear()
    {
        _keyframes.Clear();
    }
}
