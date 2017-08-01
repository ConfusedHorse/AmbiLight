using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using AmbiLight.CrossCutting.Enums;
using AmbiLight.CrossCutting.Module;
using AmbiLight.View.Module;
using AmbiLight.ViewModel.Models.Events;
using AmbiLight.ViewModel.Models.Modes;
using Ninject;

namespace AmbiLight.View.Controls
{
    /// <summary>
    /// Interaktionslogik für AmbiLightSubModes.xaml
    /// </summary>
    public partial class AmbiLightSubModes
    {
        private DateTime _canCollapseUntil = DateTime.Now; 
        private readonly Duration _fadeInDuration = new Duration(TimeSpan.FromMilliseconds(1000));
        private readonly Duration _fadeOutDuration = new Duration(TimeSpan.FromMilliseconds(500));
        private readonly Duration _moveDuration = new Duration(TimeSpan.FromMilliseconds(333));
        private readonly Duration _nullDuration = new Duration(TimeSpan.FromMilliseconds(0));

        private readonly AmbiLightOptions _options;
        private AmbiLightTray _ambiLightTray;
        private FrameworkElement _lastInvoker;

        public event MouseOverModeChanged MouseOverModeChanged;

        private double _maxLeft;
        private double _maxTop;

        private double _newWidth;
        private double _newHeight;

        public AmbiLightSubModes(AmbiLightOptions options)
        {
            _options = options;
            InitializeComponent();
            Opacity = 0;
            ShowActivated = false;
        }
        
        public bool InUse => IsActive || IsFocused || IsMouseOver;

        private void AmbiLightSubModes_OnLoaded(object sender, RoutedEventArgs e)
        {
            _ambiLightTray = AmbiLightKernel.Instance.Get<AmbiLightTray>();
            _ambiLightTray.MouseOverModeChanged += AmbiLightTrayOnMouseOverModeChanged;
            _options.MouseOverModeChanged += AmbiLightTrayOnMouseOverModeChanged;

            var desktopWorkingArea = SystemParameters.WorkArea;

            _maxLeft = desktopWorkingArea.Right;
            _maxTop = desktopWorkingArea.Bottom - 100 - _ambiLightTray.ActualHeight;
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
            if (sender is AmbiLightOptions)
            {
                OnMouseOverModeEnter();
            }
            else
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

        private void OnMouseOverModeEnter(MouseOverModeArgs args = null)
        {
            if (args != null && args.Mode.SubModes == null) return;

            _canCollapseUntil = DateTime.MaxValue;
            Visibility = Visibility.Visible;
            SubModes.IsEnabled = true;

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
                DataContext = args.Mode;
                AdjustSize(args.Mode);

                var targetLeft = args.PositionOfMode.X - _newWidth / 2;
                if (targetLeft > _maxLeft) targetLeft = _maxLeft;

                var positionLeftAnimation = new DoubleAnimation
                {
                    To = targetLeft,
                    Duration = Opacity > 0 ? _moveDuration : _nullDuration,
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                };
                sb.Children.Add(positionLeftAnimation);

                Storyboard.SetTargetProperty(positionLeftAnimation, new PropertyPath(LeftProperty));
                Storyboard.SetTarget(positionLeftAnimation, this);
            }

            sb.Begin();
        }

        private void AdjustSize(IAmbiLightMode argsMode)
        {
            const double tileSize = 100d;
            var count = argsMode.SubModes.Count;

            _newWidth = tileSize * count;
            _newHeight = tileSize;

            var sb = new Storyboard();
            var widthAnimation = new DoubleAnimation
            {
                To = _newWidth,
                Duration = _moveDuration
            };
            sb.Children.Add(widthAnimation);

            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(WidthProperty));
            Storyboard.SetTarget(widthAnimation, this);

            Height = tileSize;

            var desktopWorkingArea = SystemParameters.WorkArea;

            _maxLeft = desktopWorkingArea.Right - _newWidth;
            _maxTop = desktopWorkingArea.Bottom - _newHeight - _ambiLightTray.ActualHeight;

            sb.Begin();
        }

        private void OnMouseOverModeLeave()
        {
            if (IsMouseOver || _options.InUse || _lastInvoker.IsMouseOver) return;
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

        private void SbCompleted(object sender, EventArgs e)
        {
            if (DateTime.Now < _canCollapseUntil) return;

            Visibility = Visibility.Collapsed;
        }

        private void AmbiLightSubModes_OnMouseLeave(object sender, MouseEventArgs e)
        {
            OnMouseOverModeLeave();
        }

        private void AmbiLightSubModes_OnMouseEnter(object sender, MouseEventArgs e)
        {
            OnMouseOverModeEnter(null);
        }

        private void ModeTemplate_OnClick(object sender, RoutedEventArgs e)
        {
            var mode = (sender as Button)?.DataContext as IAmbiLightMode;
            if (mode == null) return;
            ViewModelLocator.Instance.AmbiLightViewModel.CurrentMode = mode;
        }

        private void AmbiLightSubModes_OnDeactivated(object sender, EventArgs e)
        {
            if (_options.InUse || _lastInvoker.IsMouseOver) return;

            Visibility = Visibility.Hidden;
            SubModes.IsEnabled = false;
        }

        private void ModeGrid_OnMouseEnter(object sender, MouseEventArgs e)
        {
            var modeGrid = sender as FrameworkElement;
            var dataContext = modeGrid?.DataContext as IAmbiLightMode;
            if (dataContext == null) return;

            var position = modeGrid.TransformToAncestor(this)
                .Transform(new Point(modeGrid.ActualWidth / 2, 0d)) + new Vector(Left, Top);
            MouseOverModeChanged?.Invoke(sender, new MouseOverModeArgs(dataContext, MouseOver.Enter, position));
        }

        private void ModeGrid_OnMouseLeave(object sender, MouseEventArgs e)
        {
            MouseOverModeChanged?.Invoke(sender, new MouseOverModeArgs(MouseOver.Leave));
        }
    }
}
