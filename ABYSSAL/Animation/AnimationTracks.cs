using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ABYSSAL.Animation;

public class AnimationTrack
{
    public string Name { get; set; } = "";
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsLooping { get; set; }

    public double GetProgress(TimeSpan time)
    {
        if (time < StartTime) return 0;
        if (time > EndTime)
        {
            if (IsLooping)
            {
                var duration = (EndTime - StartTime).TotalSeconds;
                if (duration > 0)
                {
                    var elapsed = (time - StartTime).TotalSeconds % duration;
                    return elapsed / duration;
                }
            }
            return 1;
        }

        var totalDuration = (EndTime - StartTime).TotalSeconds;
        if (totalDuration <= 0) return 1;

        return (time - StartTime).TotalSeconds / totalDuration;
    }

    public bool IsActive(TimeSpan time)
    {
        return time >= StartTime && time <= EndTime;
    }
}

public class TransformTrack : AnimationTrack
{
    public Point3D StartPosition { get; set; }
    public Point3D EndPosition { get; set; }
    public Vector3D StartRotation { get; set; }
    public Vector3D EndRotation { get; set; }
    public double StartScale { get; set; } = 1.0;
    public double EndScale { get; set; } = 1.0;

    public Point3D GetPosition(TimeSpan time)
    {
        var t = EaseInOutCubic(GetProgress(time));
        return new Point3D(
            StartPosition.X + (EndPosition.X - StartPosition.X) * t,
            StartPosition.Y + (EndPosition.Y - StartPosition.Y) * t,
            StartPosition.Z + (EndPosition.Z - StartPosition.Z) * t
        );
    }

    public Vector3D GetRotation(TimeSpan time)
    {
        var t = GetProgress(time);
        return new Vector3D(
            StartRotation.X + (EndRotation.X - StartRotation.X) * t,
            StartRotation.Y + (EndRotation.Y - StartRotation.Y) * t,
            StartRotation.Z + (EndRotation.Z - StartRotation.Z) * t
        );
    }

    public double GetScale(TimeSpan time)
    {
        var t = GetProgress(time);
        return StartScale + (EndScale - StartScale) * t;
    }

    private static double EaseInOutCubic(double t)
    {
        return t < 0.5 ? 4 * t * t * t : 1 - Math.Pow(-2 * t + 2, 3) / 2;
    }
}

public class MechanicalAnimation
{
    private readonly List<TransformTrack> _componentTracks = new();

    public void AddComponentTrack(string name, TransformTrack track)
    {
        track.Name = name;
        _componentTracks.Add(track);
    }

    public void Update(TimeSpan time, IEnumerable<(string Name, Transform3D Transform)> components)
    {
        foreach (var track in _componentTracks)
        {
            var component = components.FirstOrDefault(c => c.Name == track.Name);
            if (component.Transform != null && track.IsActive(time))
            {
                var position = track.GetPosition(time);
                var scale = track.GetScale(time);

                if (component.Transform is TranslateTransform3D translate)
                {
                    translate.OffsetX = position.X;
                    translate.OffsetY = position.Y;
                    translate.OffsetZ = position.Z;
                }
            }
        }
    }
}

public class CreatureAnimation
{
    private readonly List<CreatureBehavior> _behaviors = new();

    public void AddBehavior(CreatureBehavior behavior)
    {
        _behaviors.Add(behavior);
    }

    public void Update(TimeSpan time)
    {
        foreach (var behavior in _behaviors)
        {
            behavior.Update(time);
        }
    }
}

public class CreatureBehavior
{
    private readonly Point3D _startPosition;
    private readonly Vector3D _direction;
    private readonly double _speed;
    private readonly double _phase;

    public TranslateTransform3D Transform { get; set; } = null!;

    public CreatureBehavior(Point3D start, Vector3D direction, double speed, double phase)
    {
        _startPosition = start;
        _direction = direction;
        _speed = speed;
        _phase = phase;
    }

    public void Update(TimeSpan time)
    {
        if (Transform == null) return;

        var seconds = time.TotalSeconds;
        var progress = (_speed * seconds + _phase) % 100;

        var x = _startPosition.X + _direction.X * progress;
        var y = _startPosition.Y + Math.Sin(seconds * 0.5 + _phase) * 2;
        var z = _startPosition.Z + _direction.Z * progress;

        if (x < -150) x += 300;
        if (x > 150) x -= 300;
        if (z < -150) z += 300;
        if (z > 150) z -= 300;

        Transform.OffsetX = x;
        Transform.OffsetY = y;
        Transform.OffsetZ = z;
    }
}
