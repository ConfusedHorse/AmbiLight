using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Media.Imaging;
using AmbiLight.CrossCutting.Helpers;
using AmbiLight.Enums;
using AmbiLight.ViewModel.Module;
using AmbiLight.ViewModel.Properties;
using GalaSoft.MvvmLight;

namespace AmbiLight.ViewModel.Models.Modes.CustomModes.Miscellaneous
{
    public class AmbiLightSaturatedMode : ViewModelBase, IAmbiLightMode
    {
        #region Fields
        
        private readonly ScreenHelper _screenHelper;
        private readonly int _horizontalLedCount;
        private readonly int _verticalLedCount;
        private readonly double _saturation;
        private bool _isActive;

        #endregion

        #region Properties

        public string Name { get; }

        public string ToolTip { get; }

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                RaisePropertyChanged();
            }
        }

        public bool HasColorPicker { get; }

        public bool IsModeGroup { get; }

        public BitmapImage ImageSource { get; }

        public TimeSpan RecommendedInterval { get; set; }

        public Color Color { get; set; }

        public bool WasLastActive { get; set; }

        public string GroupName { get; }
        
        public ObservableCollection<IAmbiLightMode> SubModes { get; }

        #endregion

        public AmbiLightSaturatedMode(ScreenHelper screenHelper)
        {
            Name = Resources.AmbiLightSaturatedName;
            ToolTip = Resources.AmbiLightSaturatedToolTip;
            ImageSource = "monitor64.png".ToBitmapFrom(Local.ImagePath);
            GroupName = "Miscellaneous";
            HasColorPicker = false;
            IsModeGroup = false;

            _screenHelper = screenHelper;
            _horizontalLedCount = Settings.Default.HorizontalLedCount;
            _verticalLedCount = Settings.Default.VerticalLedCount;
            _saturation = Settings.Default.Saturation;
        }

        #region Public Methods

        public Color[] GetColors()
        {
            var colors = _screenHelper.GetEmptyColorArray();

            _screenHelper.CaptureColorArray(Orientation.Right).CopyTo(colors, 0);
            _screenHelper.CaptureColorArray(Orientation.Top).CopyTo(colors, _verticalLedCount);
            _screenHelper.CaptureColorArray(Orientation.Left).CopyTo(colors, _verticalLedCount + _horizontalLedCount);

            return colors.AlterSaturationBy(_saturation);
        }

        #endregion 
    }
}