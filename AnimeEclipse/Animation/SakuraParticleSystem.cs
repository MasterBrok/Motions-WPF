using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace AnimeEclipse.Animation
{
    public class SakuraParticleSystem : IDisposable
    {
        private readonly Canvas _canvas;
        private readonly List<Petal> _petals = new();
        private readonly Random _random = new();
        private double _intensity;

        public SakuraParticleSystem(Canvas layer)
        {
            _canvas = layer;
            for (int i = 0; i < 40; i++)
            {
                var petal = new Petal
                {
                    X = _random.NextDouble() * 1920,
                    Y = _random.NextDouble() * 1080,
                    Vx = -0.3 - _random.NextDouble() * 0.6,
                    Vy = 0.15 + _random.NextDouble() * 0.5,
                    Rotation = _random.NextDouble() * 360,
                    RotationSpeed = (_random.NextDouble() - 0.5) * 3,
                    Size = 4 + _random.NextDouble() * 8,
                    Depth = 0.4 + _random.NextDouble() * 0.6,
                    Phase = _random.NextDouble() * Math.PI * 2,
                    Visual = CreatePetalVisual()
                };
                _petals.Add(petal);
                _canvas.Children.Add(petal.Visual);
            }
        }

        private Ellipse CreatePetalVisual()
        {
            return new Ellipse
            {
                Fill = new RadialGradientBrush
                {
                    GradientStops = new GradientStopCollection
                    {
                        new(Color.FromArgb(220, 255, 145, 200), 0),
                        new(Color.FromArgb(160, 255, 145, 200), 0.6),
                        new(Color.FromArgb(0, 255, 145, 200), 1)
                    }
                },
                Effect = new DropShadowEffect { Color = Color.FromRgb(255, 145, 200), BlurRadius = 6, ShadowDepth = 0, Opacity = 0.5 }
            };
        }

        public void Update(TimelineManager timeline)
        {
            var ms = timeline.Progress * 30000;

            _intensity = 1.0;
            if (timeline.IsInRange(10000, 15000))
            {
                var p = timeline.GetSceneProgress(10000, 5000);
                _intensity = 1.0 + p * 2.0;
            }
            else if (timeline.IsInRange(15000, 21000))
            {
                _intensity = 3.0;
            }
            else if (timeline.IsInRange(21000, 30000))
            {
                _intensity = 0.6;
            }

            foreach (var petal in _petals)
            {
                petal.Phase += 0.02 * _intensity;
                petal.X += (petal.Vx + Math.Sin(petal.Phase) * 0.4) * _intensity * petal.Depth;
                petal.Y += petal.Vy * _intensity * petal.Depth;
                petal.Rotation += petal.RotationSpeed * _intensity;

                if (petal.X < -20) petal.X = 1940;
                if (petal.X > 1940) petal.X = -20;
                if (petal.Y > 1100) petal.Y = -20;

                petal.UpdateVisual();
            }
        }

        public void Dispose()
        {
            foreach (var p in _petals)
            {
                _canvas.Children.Remove(p.Visual);
            }
            _petals.Clear();
        }

        private class Petal
        {
            public double X, Y, Vx, Vy, Rotation, RotationSpeed, Size, Depth, Phase;
            public UIElement Visual = null!;

            public void UpdateVisual()
            {
                if (Visual is FrameworkElement fe)
                {
                    fe.Width = Size;
                    fe.Height = Size * 0.7;
                    Canvas.SetLeft(fe, X);
                    Canvas.SetTop(fe, Y);
                    fe.Opacity = 0.5 + Depth * 0.5;
                    Visual.RenderTransform = new RotateTransform(Rotation);
                }
            }
        }
    }
}
