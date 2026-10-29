using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace AnimeEclipse.Animation
{
    public class EffectsArtist
    {
        public List<Line> SpeedLines = new();
        public List<Ellipse> EnergyRings = new();
        public List<UIElement> SealMarks = new();
        public List<Line> RadialBurst = new();
        private readonly Canvas _host;
        private readonly Random _random = new(7);

        private readonly Color _violet = Color.FromRgb(154, 77, 255);
        private readonly Color _silver = Color.FromRgb(220, 231, 255);

        public EffectsArtist(Canvas host)
        {
            _host = host;
        }

        public void BuildSpeedLines(double w, double h)
        {
            for (int i = 0; i < 28; i++)
            {
                var y = h * 0.15 + _random.NextDouble() * h * 0.7;
                var len = 80 + _random.NextDouble() * 260;
                var line = new Line
                {
                    X1 = 0, Y1 = 0, X2 = len, Y2 = 0,
                    Stroke = new SolidColorBrush(Color.FromArgb(
                        (byte)(40 + _random.Next(0, 70)), 220, 231, 255)),
                    StrokeThickness = 1 + _random.NextDouble() * 2,
                    Opacity = 0,
                    RenderTransform = new TranslateTransform(0, y)
                };
                SpeedLines.Add(line);
                _host.Children.Add(line);
            }
        }

        public void BuildEnergyRings(double cx, double cy)
        {
            for (int i = 0; i < 4; i++)
            {
                var ring = new Ellipse
                {
                    Width = 60 + i * 50,
                    Height = 60 + i * 50,
                    Stroke = new SolidColorBrush(Color.FromArgb(200, 154, 77, 255)),
                    StrokeThickness = 2.5 - i * 0.4,
                    Opacity = 0,
                    Effect = new DropShadowEffect { Color = _violet, BlurRadius = 12, ShadowDepth = 0, Opacity = 0.8 }
                };
                Canvas.SetLeft(ring, cx - ring.Width / 2);
                Canvas.SetTop(ring, cy - ring.Height / 2);
                EnergyRings.Add(ring);
                _host.Children.Add(ring);
            }
        }

        public void BuildSealMarks(double cx, double cy, double radius)
        {
            // ancient seal: hexagram + runic ticks around sword point
            for (int i = 0; i < 6; i++)
            {
                var angle = Math.PI * 2 * i / 6 - Math.PI / 2;
                var mark = new Line
                {
                    X1 = cx + Math.Cos(angle) * (radius - 14),
                    Y1 = cy + Math.Sin(angle) * (radius - 14),
                    X2 = cx + Math.Cos(angle) * (radius + 14),
                    Y2 = cy + Math.Sin(angle) * (radius + 14),
                    Stroke = new SolidColorBrush(_silver),
                    StrokeThickness = 2,
                    Opacity = 0,
                    Effect = new DropShadowEffect { Color = _violet, BlurRadius = 8, ShadowDepth = 0, Opacity = 0.9 }
                };
                SealMarks.Add(mark);
                _host.Children.Add(mark);
            }
            // circle
            var circle = new Ellipse
            {
                Width = radius * 2, Height = radius * 2,
                Stroke = new SolidColorBrush(Color.FromArgb(160, 220, 231, 255)),
                StrokeThickness = 1.2,
                StrokeDashArray = new DoubleCollection { 6, 5 },
                Opacity = 0
            };
            Canvas.SetLeft(circle, cx - radius);
            Canvas.SetTop(circle, cy - radius);
            SealMarks.Add(null!); // placeholder keeps ordering simple
            _host.Children.Add(circle);
            SealMarks[^1] = circle;
        }

        public void BuildRadialBurst(double cx, double cy, double w, double h)
        {
            for (int i = 0; i < 16; i++)
            {
                var angle = Math.PI * 2 * i / 16 + _random.NextDouble() * 0.2;
                var len = 100 + _random.NextDouble() * 180;
                var line = new Line
                {
                    X1 = cx + Math.Cos(angle) * 30,
                    Y1 = cy + Math.Sin(angle) * 30,
                    X2 = cx + Math.Cos(angle) * (30 + len),
                    Y2 = cy + Math.Sin(angle) * (30 + len),
                    Stroke = new SolidColorBrush(Color.FromArgb(180, 255, 255, 255)),
                    StrokeThickness = 1.5,
                    Opacity = 0
                };
                RadialBurst.Add(line);
                _host.Children.Add(line);
            }
        }

        // per-frame updates driven from SceneRenderer
        public void UpdateSpeedLines(TimelineManager timeline, double speedFactor)
        {
            if (!timeline.IsInRange(14000, 21500))
            {
                foreach (var l in SpeedLines) l.Opacity = 0;
                return;
            }
            var ms = timeline.Progress * 30000;
            for (int i = 0; i < SpeedLines.Count; i++)
            {
                var l = SpeedLines[i];
                var tr = (TranslateTransform)l.RenderTransform;
                tr.X -= (18 + i % 5 * 6) * speedFactor;
                if (tr.X < -320) tr.X = 1980;
                l.Opacity = Math.Min(0.8, speedFactor) * (0.4 + (i % 3) * 0.2);
            }
        }

        public void UpdateEnergyRings(TimelineManager timeline)
        {
            var active = timeline.IsInRange(10500, 15000);
            if (!active)
            {
                foreach (var r in EnergyRings) r.Opacity = 0;
                return;
            }
            var p = timeline.GetSceneProgress(10500, 4500);
            for (int i = 0; i < EnergyRings.Count; i++)
            {
                var r = EnergyRings[i];
                var local = Math.Clamp(p * 1.4 - i * 0.18, 0, 1);
                var scale = 1 + local * 2.2;
                var baseW = 60 + i * 50;
                r.Width = baseW * scale;
                r.Height = baseW * scale;
                Canvas.SetLeft(r, Canvas.GetLeft(r) + (baseW - r.Width) / 2);
                Canvas.SetTop(r, Canvas.GetTop(r) + (baseW - r.Height) / 2);
                r.Opacity = (1 - local) * 0.9;
                r.StrokeThickness = 2.5 - i * 0.4;
            }
        }

        public void UpdateSeals(TimelineManager timeline)
        {
            var active = timeline.IsInRange(11500, 15000);
            var p = active ? timeline.GetSceneProgress(11500, 3500) : 0;
            foreach (var m in SealMarks)
            {
                if (m == null) continue;
                m.Opacity = active ? Math.Clamp(p * 2.5, 0, 0.95) : 0;
            }
        }

        public void UpdateRadialBurst(TimelineManager timeline)
        {
            var active = timeline.IsInRange(14800, 15600);
            if (!active)
            {
                foreach (var l in RadialBurst) l.Opacity = 0;
                return;
            }
            var ms = timeline.Progress * 30000;
            var p = timeline.InverseLerp(14800, 15600, ms);
            foreach (var l in RadialBurst)
            {
                l.Opacity = (1 - p) * 0.9;
                // stretch outward
                l.X2 += (l.X2 - l.X1) * 0.06;
                l.Y2 += (l.Y2 - l.Y1) * 0.06;
            }
        }
    }
}
