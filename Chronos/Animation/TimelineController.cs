namespace Chronos.Animation;

public class TimelineController
{
    private TimeSpan _currentTime;
    private TimeSpan _duration = TimeSpan.FromSeconds(30);

    public event EventHandler? SeekCompleted;
    public event EventHandler? TimelineUpdated;

    public TimeSpan CurrentTime => _currentTime;
    public TimeSpan Duration => _duration;

    public int CurrentScene => _currentTime.TotalSeconds switch
    {
        < 5 => 0,
        < 10 => 1,
        < 16 => 2,
        < 22 => 3,
        < 27 => 4,
        _ => 5
    };

    public double GetScenePhase(int scene)
    {
        double start = scene switch
        {
            0 => 0,
            1 => 5,
            2 => 10,
            3 => 16,
            4 => 22,
            5 => 27,
            _ => 30
        };
        double end = scene switch
        {
            0 => 5,
            1 => 10,
            2 => 16,
            3 => 22,
            4 => 27,
            5 => 30,
            _ => 30
        };

        if (_currentTime.TotalSeconds <= start) return 0;
        if (_currentTime.TotalSeconds >= end) return 1;

        return (_currentTime.TotalSeconds - start) / (end - start);
    }

    public void Update(TimeSpan elapsed)
    {
        _currentTime = elapsed > _duration ? _duration : elapsed;
        TimelineUpdated?.Invoke(this, EventArgs.Empty);
    }

    public void Seek(TimeSpan time)
    {
        _currentTime = time > _duration ? _duration : time;
        SeekCompleted?.Invoke(this, EventArgs.Empty);
    }

    public static double EaseInOut(double t)
    {
        return t < 0.5 ? 2 * t * t : 1 - Math.Pow(-2 * t + 2, 2) / 2;
    }

    public static double EaseIn(double t)
    {
        return t * t * t;
    }

    public static double EaseOut(double t)
    {
        return 1 - Math.Pow(1 - t, 3);
    }

    public static double EaseOutElastic(double t)
    {
        double c4 = 2 * Math.PI / 3;
        return t == 0 ? 0 : t == 1 ? 1 : Math.Pow(2, -10 * t) * Math.Sin((t * 10 - 0.75) * c4) + 1;
    }

    public static double EaseInBack(double t)
    {
        double c1 = 1.70158;
        double c3 = c1 + 1;
        return c3 * t * t * t - c1 * t * t;
    }
}