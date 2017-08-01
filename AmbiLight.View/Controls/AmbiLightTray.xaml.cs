using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AmbiLight.CrossCutting.Enums;
using AmbiLight.View.Module;
using AmbiLight.ViewModel.Models.Events;
using AmbiLight.ViewModel.Models.Modes;

namespace AmbiLight.View.Controls
{
    /// <summary>
    /// Interaktionslogik für AmbiLightTray.xaml
    /// </summary>
    public partial class AmbiLightTray
    {
        private readonly AmbiLightOptions _options;
        private readonly AmbiLightSubModes _ambiLightSubModes;
        public event MouseOverModeChanged MouseOverModeChanged;

        public AmbiLightTray(AmbiLightSubModes ambiLightSubModes, AmbiLightOptions options)
        {
            _ambiLightSubModes = ambiLightSubModes;
            _options = options;
            InitializeComponent();
        }

        private void AmbiLightTray_OnLoaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - Width;
            Top = desktopWorkingArea.Bottom - Height;

            _ambiLightSubModes.ForceRendering();
            _options.ForceRendering();
        }

        public void ShowHide()
        {
            if (Visibility == Visibility.Visible)
            {
                Visibility = Visibility.Hidden;
                Interaction.IsEnabled = false;

                _ambiLightSubModes.Visibility = Visibility.Hidden;
                _options.Visibility = Visibility.Hidden;
            }
            else
            {
                Visibility = Visibility.Visible;
                Interaction.IsEnabled = true;

                if (!IsVisible)
                {
                    Show();
                }

                if (WindowState == WindowState.Minimized)
                {
                    WindowState = WindowState.Normal;
                }

                Activate();
                Topmost = true; 
                Topmost = false;
                Focus();        
            }
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

        private void ModeTemplate_OnClick(object sender, RoutedEventArgs e)
        {
            var mode = (sender as Button)?.DataContext as IAmbiLightMode;
            if (mode == null) return;
            ViewModelLocator.Instance.AmbiLightViewModel.CurrentMode = mode;
        }

        private void AmbiLightTray_OnDeactivated(object sender, EventArgs e)
        {
            if (_options.InUse || _ambiLightSubModes.InUse) return;

            Visibility = Visibility.Hidden;
            Interaction.IsEnabled = false;
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
