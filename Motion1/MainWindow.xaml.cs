using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Motion1
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
            ApplyPerformanceProfile();

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

        private void ApplyPerformanceProfile()
        {
            // Tier 0/1 devices struggle with layered compositing, so reduce overdraw.
            int renderTier = RenderCapability.Tier >> 16;

            if (renderTier < 2)
            {
                ParticleLayer.Visibility = Visibility.Collapsed;
            }
        }
    }
}