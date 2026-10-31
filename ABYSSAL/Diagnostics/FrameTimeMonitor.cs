using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace ABYSSAL.Diagnostics;

public class FrameTimeMonitor
{
    private readonly Queue<double> _frameTimes = new();
    private readonly int _maxSamples = 60;
    private DateTime _lastFrame;

    public double CurrentFps { get; private set; }
    public double AverageFps { get; private set; }
    public double MinFps { get; private set; } = double.MaxValue;
    public double MaxFps { get; private set; }
    public double LastFrameTime { get; private set; }

    public void Update()
    {
        var now = DateTime.Now;

        if (_lastFrame != DateTime.MinValue)
        {
            var elapsed = (now - _lastFrame).TotalMilliseconds;
            LastFrameTime = elapsed;
            CurrentFps = elapsed > 0 ? 1000.0 / elapsed : 0;

            _frameTimes.Enqueue(CurrentFps);
            if (_frameTimes.Count > _maxSamples)
            {
                _frameTimes.Dequeue();
            }

            AverageFps = _frameTimes.Average();
            MinFps = Math.Min(MinFps, CurrentFps);
            MaxFps = Math.Max(MaxFps, CurrentFps);
        }

        _lastFrame = now;
    }

    public string GetStats()
    {
        return $"FPS: {CurrentFps:F1} (Avg: {AverageFps:F1}, Min: {MinFps:F1}, Max: {MaxFps:F1})\nFrame: {LastFrameTime:F2}ms";
    }
}
