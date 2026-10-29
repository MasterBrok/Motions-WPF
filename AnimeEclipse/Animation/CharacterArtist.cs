using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace AnimeEclipse.Animation
{
    public class CharacterArtist
    {
        public Canvas Root = null!;
        public List<Path> HairStrands = new();
        public List<Path> CoatTails = new();
        public Path KatanaBlade = null!;
        public Ellipse KatanaGlow = null!;
        public Canvas KatanaGlowHost = null!;

        private readonly Color _obsidian = Color.FromRgb(10, 10, 20);
        private readonly Color _silver = Color.FromRgb(220, 231, 255);
        private readonly Color _violet = Color.FromRgb(154, 77, 255);

        public CharacterArtist(double scale = 1.0)
        {
            Build(scale);
        }

        private void Build(double scale)
        {
            Root = new Canvas
            {
                Width = 420,
                Height = 420,
                Opacity = 0
            };

            // ---- Rooftop platform ----
            var roof = new Polygon
            {
                Points = new PointCollection
                {
                    new(30, 418), new(390, 418), new(420, 420), new(420, 428),
                    new(0, 428), new(0, 420)
                },
                Fill = new SolidColorBrush(_obsidian)
            };
            Root.Children.Add(roof);

            // roof tile ridges
            for (int i = 0; i < 9; i++)
            {
                var ridge = new Line
                {
                    X1 = 40 + i * 38,
                    Y1 = 419,
                    X2 = 32 + i * 38,
                    Y2 = 428,
                    Stroke = new SolidColorBrush(Color.FromRgb(30, 32, 55)),
                    StrokeThickness = 2
                };
                Root.Children.Add(ridge);
            }

            // ---- Body: coat silhouette ----
            // Standing figure, feet at y=418, head top y=150
            var coat = new Path
            {
                Data = PathGeometry.Parse(
                    "M 190,190 " +                                     // neck left
                    "C 175,195 165,205 160,225 " +                     // left shoulder
                    "C 150,260 140,290 118,330 " +                     // coat flare left upper
                    "C 105,355 92,380 84,410 " +                       // coat flare left lower
                    "L 96,404 L 110,414 L 124,404 L 126,418 " +        // jagged hem left
                    "L 210,418 " +                                     // hem center
                    "L 294,418 " +                                     // hem right
                    "L 296,404 L 308,412 L 318,396 " +                 // jagged hem right
                    "C 322,372 310,350 300,328 " +                     // coat flare right
                    "C 288,296 276,262 268,228 " +                     // right side
                    "C 264,208 254,196 240,190 " +                     // right shoulder
                    "Z"),
                Fill = new SolidColorBrush(Color.FromRgb(12, 12, 24)),
                Stroke = new SolidColorBrush(Color.FromRgb(60, 62, 90)),
                StrokeThickness = 1.5
            };
            Root.Children.Add(coat);

            // ---- Silver coat accents ----
            var coatTrim = new Path
            {
                Data = PathGeometry.Parse(
                    "M 168,215 C 160,250 150,285 138,315 " +
                    "M 262,215 C 270,250 280,285 292,315"),
                Stroke = new SolidColorBrush(Color.FromArgb(180, 220, 231, 255)),
                StrokeThickness = 1.6,
                StrokeDashArray = new DoubleCollection { 4, 3 }
            };
            Root.Children.Add(coatTrim);

            // ---- Head ----
            var head = new Ellipse
            {
                Width = 34,
                Height = 40,
                Fill = new SolidColorBrush(Color.FromRgb(16, 16, 30))
            };
            Canvas.SetLeft(head, 193);
            Canvas.SetTop(head, 152);
            Root.Children.Add(head);

            // ---- Topknot / hair tie ----
            var topknot = new Ellipse
            {
                Width = 16,
                Height = 10,
                Fill = new SolidColorBrush(_obsidian)
            };
            Canvas.SetLeft(topknot, 195);
            Canvas.SetTop(topknot, 142);
            Root.Children.Add(topknot);

            // silver hair-ornament
            var ornament = new Line
            {
                X1 = 224, Y1 = 150, X2 = 244, Y2 = 144,
                Stroke = new SolidColorBrush(_silver),
                StrokeThickness = 2,
                StrokeEndLineCap = PenLineCap.Round
            };
            Root.Children.Add(ornament);

            // ---- Flowing hair strands (animated) ----
            HairStrands.Add(CreateHair(
                "M 200,168 C 160,190 130,240 122,300 C 118,340 108,370 90,392 C 110,380 128,350 134,310 C 142,250 158,205 204,182 Z"));
            HairStrands.Add(CreateHair(
                "M 208,170 C 240,192 258,238 264,290 C 268,330 280,362 300,384 C 276,376 258,344 252,304 C 246,258 238,220 206,178 Z"));
            HairStrands.Add(CreateHair(
                "M 196,170 C 176,200 170,240 178,276 C 182,262 186,240 192,222 C 192,204 194,186 200,174 Z"));
            foreach (var h in HairStrands) Root.Children.Add(h);

            // ---- Coat tails (animated) ----
            CoatTails.Add(CreateCoatTail(
                "M 150,320 C 130,350 108,378 84,398 C 128,392 152,364 164,336 Z"));
            CoatTails.Add(CreateCoatTail(
                "M 292,330 C 348,372 360,394 356,412 C 330,404 300,380 288,352 Z"));

            // ---- Arm + Katana ----
            var arm = new Line
            {
                X1 = 232, Y1 = 222, X2 = 282, Y2 = 280,
                Stroke = new SolidColorBrush(Color.FromRgb(14, 14, 26)),
                StrokeThickness = 11,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round
            };
            Root.Children.Add(arm);

            var hand = new Ellipse
            {
                Width = 10, Height = 10,
                Fill = new SolidColorBrush(Color.FromRgb(24, 24, 40))
            };
            Canvas.SetLeft(hand, 272);
            Canvas.SetTop(hand, 276);
            Root.Children.Add(hand);

            // Katana group: rotated around grip point (280,282)
            KatanaGlowHost = new Canvas();
            var katanaGroup = new Canvas
            {
                RenderTransform = new RotateTransform(-38, 282, 282)
            };

            KatanaGlow = new Ellipse
            {
                Width = 90,
                Height = 90,
                Fill = new RadialGradientBrush
                {
                    GradientStops = new GradientStopCollection
                    {
                        new GradientStop(Color.FromArgb(0, 230, 39, 72), 1),
                        new GradientStop(Color.FromArgb(64, 230, 39, 72), 0.6),
                        new GradientStop(Color.FromArgb(64, 230, 39, 72), 0)
                    }
                },
                Opacity = 0.55
            };
            Canvas.SetLeft(KatanaGlow, 282 - 45 + 55);
            Canvas.SetTop(KatanaGlow, 282 - 45);
            KatanaGlowHost.Children.Add(KatanaGlow);
            katanaGroup.Children.Add(KatanaGlowHost);

            // blade: long thin shape with silver->violet gradient
            KatanaBlade = new Path
            {
                Data = PathGeometry.Parse(
                    "M 282,282 L 400,164 L 408,170 L 292,290 Z"),
                Fill = new LinearGradientBrush(
                    Color.FromRgb(154, 77, 255),
                    Color.FromRgb(220, 231, 255),
                    new Point(0, 0), new Point(1, 1)),
                Effect = new DropShadowEffect { Color = _violet, BlurRadius = 18, ShadowDepth = 0, Opacity = 0.85 }
            };
            katanaGroup.Children.Add(KatanaBlade);

            // guard + grip
            var guard = new Rectangle
            {
                Width = 4, Height = 18,
                Fill = new SolidColorBrush(_silver),
                RenderTransform = new RotateTransform(-45)
            };
            Canvas.SetLeft(guard, 276);
            Canvas.SetTop(guard, 276);
            katanaGroup.Children.Add(guard);

            var grip = new Line
            {
                X1 = 276, Y1 = 288, X2 = 262, Y2 = 302,
                Stroke = new SolidColorBrush(Color.FromRgb(40, 30, 60)),
                StrokeThickness = 7,
                StrokeEndLineCap = PenLineCap.Round
            };
            katanaGroup.Children.Add(grip);
            Root.Children.Add(katanaGroup);

            Root.RenderTransform = new ScaleTransform(scale, scale);
        }

        private Path CreateHair(string data)
        {
            return new Path
            {
                Data = PathGeometry.Parse(data),
                Fill = new SolidColorBrush(Color.FromRgb(18, 16, 34)),
                Stroke = new SolidColorBrush(Color.FromArgb(120, 154, 77, 255)),
                StrokeThickness = 0.8
            };
        }

        private Path CreateCoatTail(string data)
        {
            var p = new Path
            {
                Data = PathGeometry.Parse(data),
                Fill = new SolidColorBrush(Color.FromRgb(12, 12, 24))
            };
            CoatTails.Add(p);
            Root.Children.Add(p);
            return p;
        }

        // Secondary motion: sway hair and coat with wind phase
        public void AnimateWind(double ms, double intensity)
        {
            var t = ms * 0.001;
            for (int i = 0; i < HairStrands.Count; i++)
            {
                var angle = Math.Sin(t * 1.6 + i * 1.3) * 2.2 * intensity;
                var p = HairStrands[i];
                p.RenderTransform = new RotateTransform(angle, 200, 170);
            }
            for (int i = 0; i < CoatTails.Count; i++)
            {
                var angle = Math.Sin(t * 1.4 + i * 2.1) * 2.8 * intensity;
                CoatTails[i].RenderTransform = new RotateTransform(angle, 200, 300);
            }
            // katana energy pulse
            var pulse = 0.45 + Math.Sin(t * 2.4) * 0.25;
            KatanaGlow.Opacity = Math.Clamp(pulse * intensity, 0, 1);
        }
    }
}
