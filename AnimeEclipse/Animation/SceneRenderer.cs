using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace AnimeEclipse.Animation
{
    public class SceneRenderer
    {
        private Canvas _bgLayer = null!, _midLayer = null!, _fgLayer = null!, _effectLayer = null!, _textLayer = null!;
        private double _w = 1920, _h = 1080;

        private Path _moonPath = null!;
        private Path _cityFar = null!;
        private Path _cityNear = null!;
        private Path _slashPath = null!;
        private Path _slashCore = null!;
        private TextBlock _mainTitle = null!;
        private TextBlock _subtitleText = null!;

        private CharacterArtist _hero = null!;
        private EffectsArtist _fx = null!;
        private readonly List<Path> _afterimages = new();

        public void Initialize(Window window)
        {
            _bgLayer = (Canvas)window.FindName("BackgroundLayer")!;
            _midLayer = (Canvas)window.FindName("MidgroundLayer")!;
            _fgLayer = (Canvas)window.FindName("ForegroundLayer")!;
            _effectLayer = (Canvas)window.FindName("EffectLayer")!;
            _textLayer = (Canvas)window.FindName("TypographyLayer")!;

            _w = window.ActualWidth > 0 ? window.ActualWidth : 1920;
            _h = window.ActualHeight > 0 ? window.ActualHeight : 1080;

            CreateMoon();
            CreateCityLayers();
            CreateCharacter();
            CreateSlashPaths();
            CreateTypography();

            _fx = new EffectsArtist(_effectLayer);
            _fx.BuildSpeedLines(_w, _h);
            _fx.BuildEnergyRings(_w * 0.5, _h * 0.52);
            _fx.BuildSealMarks(_w * 0.5, _h * 0.42, 190);
            _fx.BuildRadialBurst(_w * 0.5, _h * 0.5, _w, _h);
        }

        // ---------- construction ----------

        private void CreateMoon()
        {
            var r = _h * 0.34;
            var moonFill = new RadialGradientBrush
            {
                GradientStops = new GradientStopCollection
                {
                    new GradientStop(Color.FromRgb(240, 80, 100), 0),
                    new GradientStop(Color.FromRgb(140, 20, 45), 1)
                },
                Center = new Point(0.4, 0.35),
                GradientOrigin = new Point(0.4, 0.35),
                RadiusX = 0.9,
                RadiusY = 0.9
            };
            _moonPath = new Path
            {
                Data = new EllipseGeometry(new Point(r, r), r, r),
                Fill = moonFill,
                Effect = new DropShadowEffect { Color = Color.FromRgb(230, 39, 72), BlurRadius = 110, ShadowDepth = 0, Opacity = 0.65 },
                Opacity = 0
            };
            Canvas.SetLeft(_moonPath, _w * 0.58 - r);
            Canvas.SetTop(_moonPath, _h * 0.33 - r);
            _bgLayer.Children.Add(_moonPath);
        }

        private void CreateCityLayers()
        {
            _cityFar = BuildCity(_h * 0.30, 26, Color.FromRgb(14, 18, 38), 4);
            Canvas.SetTop(_cityFar, _h - _h * 0.30 - 4);
            _bgLayer.Children.Add(_cityFar);

            _cityNear = BuildCity(_h * 0.42, 18, Color.FromRgb(6, 6, 14), 1);
            Canvas.SetTop(_cityNear, _h - _h * 0.42 - 2);
            _bgLayer.Children.Add(_cityNear);
        }

        private Path BuildCity(double maxH, int towers, Color fill, double blur)
        {
            var rnd = new Random(maxH.GetHashCode() ^ towers);
            var fig = new PathFigure { StartPoint = new Point(0, maxH) };
            double x = 0;
            for (int i = 0; i <= towers; i++)
            {
                var bw = _w / towers * (0.5 + rnd.NextDouble() * 0.9);
                var bh = maxH * (0.25 + rnd.NextDouble() * 0.75);
                x += bw;
                var top = maxH - bh;
                fig.Segments.Add(new LineSegment(new Point(x - bw * 0.5, top), true));
                fig.Segments.Add(new LineSegment(new Point(x - bw * 0.5, top - bh), true));
                fig.Segments.Add(new LineSegment(new Point(x + bw * 0.15, top - bh), true));
                fig.Segments.Add(new LineSegment(new Point(x + bw * 0.6, top), true));
            }
            fig.Segments.Add(new LineSegment(new Point(_w, maxH), true));
            fig.IsClosed = true;
            var geo = new PathGeometry();
            geo.Figures.Add(fig);
            return new Path
            {
                Data = geo,
                Fill = new SolidColorBrush(fill),
                Effect = blur > 0 ? new BlurEffect { Radius = blur } : null,
                Opacity = 0
            };
        }

        private void CreateCharacter()
        {
            _hero = new CharacterArtist(1.35);
            Canvas.SetLeft(_hero.Root, _w * 0.24);
            Canvas.SetTop(_hero.Root, _h * 0.40);
            _midLayer.Children.Add(_hero.Root);
        }

        private void CreateSlashPaths()
        {
            var fig1 = new PathFigure { StartPoint = new Point(_w * 0.06, _h * 0.86) };
            fig1.Segments.Add(new QuadraticBezierSegment(new Point(_w * 0.42, _h * 0.10), new Point(_w * 0.97, _h * 0.52), true));
            fig1.Segments.Add(new QuadraticBezierSegment(new Point(_w * 0.46, _h * 0.28), new Point(_w * 0.06, _h * 0.86), true));
            var outer = new PathGeometry();
            outer.Figures.Add(fig1);

            _slashPath = new Path
            {
                Data = outer,
                Fill = new LinearGradientBrush(
                    Color.FromArgb(0, 154, 77, 255),
                    Color.FromRgb(230, 39, 72),
                    new Point(0, 1), new Point(1, 0)),
                Opacity = 0
            };
            _effectLayer.Children.Add(_slashPath);

            var fig2 = new PathFigure { StartPoint = new Point(_w * 0.06, _h * 0.86) };
            fig2.Segments.Add(new QuadraticBezierSegment(new Point(_w * 0.42, _h * 0.10), new Point(_w * 0.97, _h * 0.50), true));
            var coreGeo = new PathGeometry();
            coreGeo.Figures.Add(fig2);

            _slashCore = new Path
            {
                Data = coreGeo,
                Stroke = new LinearGradientBrush(
                    Color.FromRgb(154, 77, 255),
                    Color.FromRgb(255, 255, 255),
                    new Point(0, 0), new Point(1, 1)),
                StrokeThickness = 5,
                StrokeEndLineCap = PenLineCap.Round,
                Opacity = 0,
                Effect = new DropShadowEffect { Color = Colors.White, BlurRadius = 30, ShadowDepth = 0, Opacity = 1 }
            };
            _effectLayer.Children.Add(_slashCore);
        }

        private void CreateTypography()
        {
            _mainTitle = new TextBlock
            {
                Text = "ECLIPSE",
                FontSize = 150,
                FontFamily = new FontFamily("Yu Gothic, Meiryo, Segoe UI"),
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                TextAlignment = TextAlignment.Center,
                Width = 900,
                Opacity = 0,
                RenderTransform = new ScaleTransform(1.15, 1.15),
                RenderTransformOrigin = new Point(0.5, 0.5),
                Effect = new DropShadowEffect { Color = Color.FromRgb(230, 39, 72), BlurRadius = 45, ShadowDepth = 0, Opacity = 0.95 }
            };
            Canvas.SetLeft(_mainTitle, (_w - 900) / 2);
            Canvas.SetTop(_mainTitle, _h * 0.40);
            _textLayer.Children.Add(_mainTitle);

            _subtitleText = new TextBlock
            {
                Text = "",
                FontSize = 30,
                FontFamily = new FontFamily("Yu Gothic Light, Meiryo, Segoe UI Light"),
                FontWeight = FontWeights.Light,
                Foreground = new SolidColorBrush(Color.FromRgb(220, 231, 255)),
                TextAlignment = TextAlignment.Center,
                Width = 800,
                Opacity = 0,
                Effect = new DropShadowEffect { Color = Color.FromRgb(154, 77, 255), BlurRadius = 16, ShadowDepth = 0, Opacity = 0.85 }
            };
            Canvas.SetLeft(_subtitleText, (_w - 800) / 2);
            Canvas.SetTop(_subtitleText, _h * 0.66);
            _textLayer.Children.Add(_subtitleText);
        }

        // ---------- per-frame ----------

        public void Render(TimelineManager timeline)
        {
            var ms = timeline.Progress * 30000;

            Scene01(timeline);
            Scene02(timeline);
            Scene03(timeline);
            Scene04(timeline);
            Scene05(timeline);
            Scene06(timeline);

            _fx.UpdateSpeedLines(timeline, SpeedFactor(timeline));
            _fx.UpdateEnergyRings(timeline);
            _fx.UpdateSeals(timeline);
            _fx.UpdateRadialBurst(timeline);
        }

        private double SpeedFactor(TimelineManager t) =>
            t.IsInRange(14500, 16500) ? 3.0 : t.IsInRange(16500, 21000) ? 1.2 : 0.0;

        private void Scene01(TimelineManager t)
        {
            if (!t.IsInRange(0, 5000)) return;
            var p = t.GetSceneProgress(0, 5000);

            _moonPath.Opacity = t.EaseOut(Math.Clamp(p / 0.55, 0, 1)) * 0.95;
            _cityFar.Opacity = t.EaseInOut(Math.Clamp((p - 0.15) / 0.4, 0, 1)) * 0.8;
            _cityNear.Opacity = t.EaseInOut(Math.Clamp((p - 0.3) / 0.4, 0, 1)) * 0.95;

            // slow forward drift on city layers
            var drift = t.EaseInOut(p);
            _cityFar.RenderTransform = new TranslateTransform(-drift * 0.4, 0);
            _cityNear.RenderTransform = new TranslateTransform(-drift, 0);

            ShowSubtitle(t, "IN A WORLD BEYOND LIGHT", p, 0.35, 0.75);
        }

        private void Scene02(TimelineManager t)
        {
            if (!t.IsInRange(5000, 10000)) return;
            var p = t.GetSceneProgress(5000, 5000);

            _hero.Root.Opacity = t.EaseOut(Math.Clamp(p / 0.25, 0, 1));
            _hero.AnimateWind(t.Progress * 30000, 1.0);

            // slow push-in on character (simulated camera)
            var zoom = 1.0 + t.EaseInOut(p) * 0.06;
            _hero.Root.LayoutTransform = new ScaleTransform(zoom, zoom);

            if (p >= 0.2 && p < 0.5)
                ShowSubtitle(t, "ONE SOUL.", p, 0.2, 0.48);
            else if (p >= 0.5)
                ShowSubtitle(t, "ONE LAST STAND.", p, 0.52, 0.88);
        }

        private void Scene03(TimelineManager t)
        {
            if (!t.IsInRange(10000, 15000)) return;
            var p = t.GetSceneProgress(10000, 5000);

            _hero.AnimateWind(t.Progress * 30000, 1.5);
            _hero.Root.LayoutTransform = new ScaleTransform(1.12, 1.12);

            // sky darkens, moon intensifies
            var intensity = t.EaseInOut(Math.Clamp(p * 1.4, 0, 1));
            _moonPath.Opacity = 0.95 + Math.Sin(p * Math.PI) * 0.1;
            if (_moonPath.Effect is DropShadowEffect e) e.BlurRadius = 70 + intensity * 50;

            if (p > 0.3 && p < 0.55)
                ShowSubtitle(t, "POWER IS NOT GIVEN.", p, 0.3, 0.55);
            else if (p >= 0.55)
                ShowSubtitle(t, "IT IS AWAKENED.", p, 0.55, 0.92);
        }

        private void Scene04(TimelineManager t)
        {
            var ms = t.Progress * 30000;

            // dash phase 15.0-15.8s: hero rockets right with afterimages
            if (t.IsInRange(15000, 16600))
            {
                var dash = t.InverseLerp(15000, 15800, ms);
                _hero.Root.Opacity = 1;
                var x = _w * 0.24 + dash * _w * 0.75;
                Canvas.SetLeft(_hero.Root, x);
                _hero.KatanaBlade.RenderTransform = new RotateTransform(-38 - dash * 90, 282, 282);

                if (dash > 0.15 && _afterimages.Count < 4 && ms % 60 < 16)
                {
                    AddAfterimage(x);
                }
            }
            else if (t.IsInRange(15800, 21000))
            {
                // post-slash: hero parked right side, small
                Canvas.SetLeft(_hero.Root, _w * 0.80);
                Canvas.SetTop(_hero.Root, _h * 0.42);
                _hero.Root.Opacity = t.EaseOut(Math.Clamp((ms - 16500) / 800, 0, 1)) * 0.95;
            }
            else if (!t.IsInRange(5000, 15000))
            {
                _hero.Root.Opacity = t.IsInRange(21000, 25000) ? 0.9 * t.EaseOut(t.GetSceneProgress(21000, 2000)) : 0;
            }

            for (int i = _afterimages.Count - 1; i >= 0; i--)
            {
                var ai = _afterimages[i];
                ai.Opacity -= 0.03;
                if (ai.Opacity <= 0)
                {
                    _fgLayer.Children.Remove(ai);
                    _afterimages.RemoveAt(i);
                }
            }

            // slash reveal 15.6-16.4
            if (ms >= 15600 && ms <= 21500)
            {
                var p = t.InverseLerp(15600, 16100, ms);
                _slashPath.Opacity = t.EaseOut(Math.Clamp(p, 0, 1));
                _slashCore.Opacity = Math.Clamp(_slashPath.Opacity * 1.2, 0, 1);
                _slashCore.StrokeThickness = 10 - p * 5;

                // hold then decay
                if (ms > 16800)
                {
                    var fade = t.InverseLerp(16800, 21500, ms);
                    _slashPath.Opacity = Math.Max(0, 0.9 - fade * 0.9);
                    _slashCore.Opacity = Math.Max(0, 1 - fade * 1.2);
                }
            }
            else
            {
                _slashPath.Opacity = 0;
                _slashCore.Opacity = 0;
            }

            _mainTitle.Opacity = t.IsInRange(16400, 21000)
                ? t.EaseOut(t.InverseLerp(16400, 17000, ms)) * (1 - t.EaseIn(t.InverseLerp(20600, 21000, ms)))
                : 0;
        }

        private void Scene05(TimelineManager t)
        {
            if (!t.IsInRange(21000, 25000)) return;
            var p = t.GetSceneProgress(21000, 4000);

            _hero.Root.Opacity = t.EaseInOut(Math.Clamp(p / 0.2, 0, 1));
            Canvas.SetLeft(_hero.Root, _w * 0.68);
            Canvas.SetTop(_hero.Root, _h * 0.42);
            _hero.AnimateWind(t.Progress * 30000, 0.5);
            _moonPath.Opacity = t.EaseInOut(Math.Clamp(p / 0.3, 0, 1)) * 0.95;

            if (p > 0.1 && p < 0.45)
                ShowSubtitle(t, "ONE STRIKE.", p, 0.1, 0.45);
            else if (p >= 0.45)
                ShowSubtitle(t, "ETERNAL LEGEND.", p, 0.45, 0.95);
        }

        private void Scene06(TimelineManager t)
        {
            if (!t.IsInRange(25000, 30000)) return;
            var p = t.GetSceneProgress(25000, 5000);

            // final key visual composition
            Canvas.SetLeft(_hero.Root, _w * 0.68);
            Canvas.SetTop(_hero.Root, _h * 0.34);
            _hero.Root.Opacity = t.EaseOut(Math.Clamp(p * 3, 0, 1));
            _moonPath.Opacity = t.EaseOut(Math.Clamp(p * 2, 0, 1)) * 0.95;
            _mainTitle.Text = "ANIME ECLIPSE";
            _mainTitle.Opacity = Math.Clamp(p * 3, 0, 1) * (1 - t.EaseIn(Math.Clamp((p - 0.92) / 0.08, 0, 1)));
            var st = (ScaleTransform)_mainTitle.RenderTransform;
            st.ScaleX = st.ScaleY = 1.25 - t.EaseOut(Math.Clamp((p - 0.05) / 0.5, 0, 1)) * 0.15;

            if (p > 0.4)
                ShowSubtitle(t, "WHEN THE MOON BLEEDS, LEGENDS AWAKEN.", p, 0.4, 0.92);
            if (p > 0.94) _textLayer.Opacity = 1 - t.EaseIn((p - 0.94) / 0.06);
        }

        // ---------- helpers ----------

        private void ShowSubtitle(TimelineManager t, string text, double p, double fadeIn, double fadeOut)
        {
            if (p < fadeIn || p > fadeOut) { _subtitleText.Opacity = 0; return; }
            if (_subtitleText.Text != text) _subtitleText.Text = text;
            var inP = Math.Clamp((p - fadeIn) / 0.12, 0, 1);
            var outP = 1 - t.EaseIn(Math.Clamp((p - fadeOut + 0.15) / 0.15, 0, 1));
            _subtitleText.Opacity = t.EaseOut(inP) * outP;
        }

        private void AddAfterimage(double x)
        {
            var ghost = new Path
            {
                Data = new RectangleGeometry(new Rect(x, _h * 0.40 + 60, 90, 300)),
                Fill = new LinearGradientBrush(
                    Color.FromArgb(90, 154, 77, 255),
                    Color.FromArgb(0, 154, 77, 255),
                    new Point(0, 0), new Point(1, 0)),
                Opacity = 0.7
            };
            _fgLayer.Children.Add(ghost);
            _afterimages.Add(ghost);
        }
    }
}
