using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;

namespace AmbiLight.View.Controls
{
    /// <summary>
    ///     Interaktionslogik für AnimatedBorder.xaml
    /// </summary>
    public partial class AnimatedBorder
    {
        private bool _canCollapse = true;
        private readonly Duration _fadeDuration = new Duration(TimeSpan.FromMilliseconds(1000));

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
            "IsActive", typeof(bool), typeof(AnimatedBorder), new PropertyMetadata(default(bool)));

        public bool IsActive
        {
            get { return (bool) GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        public AnimatedBorder()
        {
            InitializeComponent();
            DependencyPropertyDescriptor.FromProperty(IsActiveProperty, typeof(AnimatedBorder))
                .AddValueChanged(this, IsActiveChanged);
            Opacity = 0d;
        }

        private void IsActiveChanged(object sender, EventArgs eventArgs)
        {
            if (IsActive)
            {
                _canCollapse = false;
                Visibility = Visibility.Visible;

                var sb = new Storyboard();
                var da = new DoubleAnimation
                {
                    To = 1d,
                    Duration = _fadeDuration
                };
                sb.Children.Add(da);
                Storyboard.SetTargetProperty(da, new PropertyPath(OpacityProperty));
                Storyboard.SetTarget(da, this);
                sb.Begin();
            }
            else
            {
                _canCollapse = true;
                Visibility = Visibility.Visible;

                var sb = new Storyboard();
                var da = new DoubleAnimation
                {
                    To = 0d,
                    Duration = _fadeDuration
                };
                sb.Children.Add(da);
                Storyboard.SetTargetProperty(da, new PropertyPath(OpacityProperty));
                Storyboard.SetTarget(da, this);
                sb.Completed += SbCompleted;
                sb.Begin();
            }
        }

        private void SbCompleted(object sender, EventArgs e)
        {
            if (!_canCollapse) return;
            
            Visibility = Visibility.Collapsed;
        }
    }
}