using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace AnimeEclipse.Animation
{
    public class ParticleSystem : IDisposable
    {
        private readonly Canvas _canvas;
        private readonly List<Particle> _particles = new();
        private readonly Random _random = new();
        private const int MaxParticles = 80;

        public ParticleSystem(Canvas layer)
        {
            _canvas = layer;
        }

        public void Update(TimelineManager timeline)
        {
            var ms = timeline.Progress * 30000;

            if (timeline.IsInRange(15000, 21000))
            {
                SpawnBurstParticles(timeline);
            }

            for (int i = _particles.Count - 1; i >= 0; i--)
            {
                var p = _particles[i];
                p.Update();
                if (p.Life <= 0)
                {
                    _canvas.Children.Remove(p.Visual);
                    _particles.RemoveAt(i);
                }
            }
        }

        private void SpawnBurstParticles(TimelineManager timeline)
        {
            if (_particles.Count < MaxParticles && _random.NextDouble() > 0.5)
            {
                var p = new Particle
                {
                    X = 960 + (_random.NextDouble() - 0.5) * 200,
                    Y = 540 + (_random.NextDouble() - 0.5) * 300,
                    Vx = (_random.NextDouble() - 0.5) * 15,
                    Vy = (_random.NextDouble() - 0.5) * 15,
                    MaxLife = 1.0,
                    Size = 2 + _random.NextDouble() * 4,
                    Visual = CreateParticleVisual()
                };
                _particles.Add(p);
                _canvas.Children.Add(p.Visual);
            }
        }

        private Ellipse CreateParticleVisual()
        {
            return new Ellipse
            {
                Width = 6,
                Height = 6,
                Fill = new RadialGradientBrush
                {
                    GradientStops = new GradientStopCollection
                    {
                        new(Color.FromRgb(220, 231, 255), 0),
                        new(Color.FromRgb(154, 77, 255), 0.7),
                        new(Color.FromRgb(154, 77, 255), 1)
                    }
                },
                Effect = new DropShadowEffect { Color = Color.FromRgb(154, 77, 255), BlurRadius = 8, ShadowDepth = 0, Opacity = 0.8 }
            };
        }

        public void Dispose()
        {
            foreach (var p in _particles)
            {
                _canvas.Children.Remove(p.Visual);
            }
            _particles.Clear();
        }

        private class Particle
        {
            public double X, Y, Vx, Vy, MaxLife, Size;
            public UIElement Visual = null!;

            public double Life { get; private set; } = 1.0;

            public void Update()
            {
                X += Vx;
                Y += Vy;
                Vx *= 0.98;
                Vy *= 0.98;
                Life -= 0.016;

                if (Visual is FrameworkElement fe)
                {
                    Canvas.SetLeft(fe, X);
                    Canvas.SetTop(fe, Y);
                    fe.Opacity = Math.Max(0, Life / MaxLife);
                }
            }
        }
    }
}
