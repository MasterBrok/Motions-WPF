using System.Windows.Media.Media3D;

namespace ABYSSAL;

public class TimelineController
{
    public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(330);
    public TimeSpan CurrentTime { get; private set; }
    public event Action? SeekCompleted;
    public event Action? TimeUpdated;

    private readonly List<CameraShot> _shots = new();
    private CameraShot? _currentShot;

    public void AddShot(CameraShot shot)
    {
        _shots.Add(shot);
        _shots.Sort((a, b) => a.StartTime.CompareTo(b.StartTime));
    }

    public CameraShot? GetCurrentShot()
    {
        return _currentShot;
    }

    public void Update(TimeSpan time)
    {
        CurrentTime = time;

        foreach (var shot in _shots)
        {
            if (time >= shot.StartTime && time <= shot.EndTime)
            {
                _currentShot = shot;
                break;
            }
        }

        TimeUpdated?.Invoke();
    }

    public void Seek(TimeSpan time)
    {
        CurrentTime = time;
        SeekCompleted?.Invoke();
    }

    public void Restart()
    {
        CurrentTime = TimeSpan.Zero;
    }

    public double GetProgress()
    {
        return CurrentTime.TotalSeconds / Duration.TotalSeconds;
    }
}
