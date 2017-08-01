using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using AmbiLight.CrossCutting.Enums;
using AmbiLight.CrossCutting.Module;
using AmbiLight.View.Module;
using AmbiLight.ViewModel.Models.Events;
using Ninject;

namespace AmbiLight.View.Controls
{
    /// <summary>
    /// Interaktionslogik für AmbiLightOptions.xaml
    /// </summary>
    public partial class AmbiLightOptions
    {
        private DateTime _canCollapseUntil = DateTime.Now;
        private readonly Duration _fadeInDuration = new Duration(TimeSpan.FromMilliseconds(1000));
        private readonly Duration _fadeOutDuration = new Duration(TimeSpan.FromMilliseconds(500));
        private readonly Duration _moveDuration = new Duration(TimeSpan.FromMilliseconds(333));
        //private readonly Duration _nullDuration = new Duration(TimeSpan.FromMilliseconds(20));
        private AmbiLightTray _ambiLightTray;
        private AmbiLightSubModes _ambiLightSubModes;
        private FrameworkElement _lastInvoker;
        public event MouseOverModeChanged MouseOverModeChanged;

        private double _maxLeft;
        private double _maxTop;

        public AmbiLightOptions()
        {
            InitializeComponent();
            Opacity = 0;
            ShowActivated = false;
        }

        public bool InUse => IsActive || IsFocused || IsMouseOver;

        private void AmbiLightOptions_OnLoaded(object sender, RoutedEventArgs e)
        {
            _ambiLightTray = AmbiLightKernel.Instance.Get<AmbiLightTray>();
            _ambiLightTray.MouseOverModeChanged += AmbiLightTrayOnMouseOverModeChanged;
            _ambiLightSubModes = AmbiLightKernel.Instance.Get<AmbiLightSubModes>();
            _ambiLightSubModes.MouseOverModeChanged += AmbiLightTrayOnMouseOverModeChanged;

            var desktopWorkingArea = SystemParameters.WorkArea;

            _maxLeft = desktopWorkingArea.Right - Width;
            _maxTop = desktopWorkingArea.Bottom - Height - _ambiLightTray.ActualHeight;
            Left = _maxLeft;
            Top = _maxTop;
        }

        public void ForceRendering()
        {
            if (!IsVisible)
            {
                Show();
            }
            Visibility = Visibility.Visible;
            Visibility = Visibility.Collapsed;
        }

        private void AmbiLightTrayOnMouseOverModeChanged(object sender, MouseOverModeArgs mouseOverModeArgs)
        {
            switch (mouseOverModeArgs.State)
            {
                case MouseOver.Enter:
                    _lastInvoker = (FrameworkElement)sender;
                    OnMouseOverModeEnter(mouseOverModeArgs);
                    break;
                case MouseOver.Leave:
                    OnMouseOverModeLeave();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnMouseOverModeEnter(MouseOverModeArgs args)
        {
            if (args != null && !args.Mode.HasColorPicker) return;

            _canCollapseUntil = DateTime.MaxValue;
            Visibility = Visibility.Visible;

            var sb = new Storyboard();
            var opacityAnimation = new DoubleAnimation
            {
                To = 1d,
                Duration = _fadeInDuration
            };
            sb.Children.Add(opacityAnimation);

            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(OpacityProperty));
            Storyboard.SetTarget(opacityAnimation, this);

            if (args != null)
            {
                var targetLeft = args.PositionOfMode.X - ActualWidth / 2;
                if (targetLeft > _maxLeft) targetLeft = _maxLeft;

                var targetTop = args.PositionOfMode.Y - ActualHeight;
                if (targetTop < 0) targetTop = 0;

                var positionLeftAnimation = new DoubleAnimation
                {
                    To = targetLeft,
                    //Duration = Opacity > 0 ? _moveDuration : _nullDuration,
                    Duration = _moveDuration,
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                };
                sb.Children.Add(positionLeftAnimation);

                var positionTopAnimation = new DoubleAnimation
                {
                    To = targetTop,
                    //Duration = Opacity > 0 ? _moveDuration : _nullDuration,
                    Duration = _moveDuration,
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                };
                sb.Children.Add(positionTopAnimation);

                Storyboard.SetTargetProperty(positionLeftAnimation, new PropertyPath(LeftProperty));
                Storyboard.SetTargetProperty(positionTopAnimation, new PropertyPath(TopProperty));
                Storyboard.SetTarget(positionLeftAnimation, this);
                Storyboard.SetTarget(positionTopAnimation, this);
            }

            sb.Begin();
        }

        private void OnMouseOverModeLeave()
        {
            if (IsMouseOver || _lastInvoker.IsMouseOver) return;
            _canCollapseUntil = DateTime.Now + _fadeOutDuration.TimeSpan;

            var sb = new Storyboard();
            var da = new DoubleAnimation
            {
                To = 0d,
                Duration = _fadeOutDuration,
                BeginTime = TimeSpan.FromMilliseconds(100)
            };
            sb.Children.Add(da);
            Storyboard.SetTargetProperty(da, new PropertyPath(OpacityProperty));
            Storyboard.SetTarget(da, this);
            sb.Completed += SbCompleted;
            sb.Begin();
        }

        private void ColorPicker_OnSelectionChanged(object sender, Color color)
        {
            ViewModelLocator.Instance.AmbiLightViewModel.SelectedColor =
                System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        private void SbCompleted(object sender, EventArgs e)
        {
            if (DateTime.Now < _canCollapseUntil) return;

            Visibility = Visibility.Collapsed;
        }

        private void AmbiLightOptions_OnMouseLeave(object sender, MouseEventArgs e)
        {
            OnMouseOverModeLeave();
        }

        private void AmbiLightOptions_OnMouseEnter(object sender, MouseEventArgs e)
        {
            MouseOverModeChanged?.Invoke(sender, new MouseOverModeArgs(null, MouseOver.Enter, new Point(-1, -1)));
            OnMouseOverModeEnter(null);
        }
    }
}
