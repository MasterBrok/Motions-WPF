using System;

namespace AnimeEclipse.Animation
{
    public class TimelineManager
    {
        private readonly double _durationMs;
        public double Progress { get; private set; }

        public double FlashIntensity { get; private set; }
        public double VignetteIntensity { get; private set; }

        public TimelineManager(double durationMs)
        {
            _durationMs = durationMs;
        }

        public void Update(double progress)
        {
            Progress = progress;
            UpdateEffects();
        }

        private void UpdateEffects()
        {
            FlashIntensity = 0;
            VignetteIntensity = 0.5;

            var ms = Progress * _durationMs;
            if (IsInRange(10000, 10500)) FlashIntensity = EaseOut(InverseLerp(10000, 10500, ms)) * 0.4;
            if (IsInRange(15000, 15100)) FlashIntensity = EaseOut(InverseLerp(15000, 15100, ms)) * 0.8;
            if (IsInRange(21000, 21200)) FlashIntensity = EaseOut(InverseLerp(21000, 21200, ms)) * 0.6;
        }

        public bool IsInRange(double start, double end)
        {
            var ms = Progress * _durationMs;
            return ms >= start && ms <= end;
        }

        public double GetSceneProgress(int sceneStartMs, int sceneDurationMs)
        {
            var ms = Progress * _durationMs;
            var sceneProgress = (ms - sceneStartMs) / sceneDurationMs;
            return Math.Clamp(sceneProgress, 0, 1);
        }

        public double EaseIn(double t) => t * t;
        public double EaseOut(double t) => 1 - (1 - t) * (1 - t);
        public double EaseInOut(double t) => t < 0.5 ? 2 * t * t : 1 - Math.Pow(-2 * t + 2, 2) / 2;
        public double EaseOutBack(double t) => 1 + 2.70158 * (float)Math.Pow(t - 1, 3) + 1.70158 * (float)Math.Pow(t - 1, 2);

        public double Lerp(double a, double b, double t) => a + (b - a) * t;
        public double InverseLerp(double a, double b, double t) => Math.Clamp((t - a) / (b - a), 0, 1);
    }
}
