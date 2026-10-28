using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Motion2
{
    public partial class MainWindow : Window
    {
        private Storyboard? _showreelStoryboard;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyPerformanceOptimizations();

            _showreelStoryboard = (Storyboard)FindResource("ShowreelStoryboard");
            _showreelStoryboard.Begin(this, true);
        }

        private void Window_Closed(object? sender, EventArgs e)
        {
            if (_showreelStoryboard is not null)
            {
                _showreelStoryboard.Stop(this);
                _showreelStoryboard.Remove(this);
            }
        }

        private void ApplyPerformanceOptimizations()
        {
            int renderTier = RenderCapability.Tier >> 16;

            if (renderTier < 2)
            {
                ParticleLayer.Visibility = Visibility.Collapsed;
            }

            RenderOptions.SetCachingHint(this, CachingHint.Cache);
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);
        }
    }
}