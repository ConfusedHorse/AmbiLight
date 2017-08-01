using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Media.Imaging;
using AmbiLight.CrossCutting.Helpers;
using AmbiLight.ViewModel.Module;
using AmbiLight.ViewModel.Properties;
using GalaSoft.MvvmLight;

namespace AmbiLight.ViewModel.Models.Modes.CustomModes.Miscellaneous
{
    public class SingleColorMode : ViewModelBase, IAmbiLightMode
    {
        #region Fields

        private readonly ScreenHelper _screenHelper;
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

        public SingleColorMode(ScreenHelper screenHelper)
        {
            Name = Resources.SingleColorName;
            ToolTip = Resources.SingleColorToolTip;
            GroupName = "Miscellaneous";
            ImageSource = "bucket64.png".ToBitmapFrom(Local.ImagePath);
            HasColorPicker = true;
            IsModeGroup = false;
            
            RecommendedInterval = TimeSpan.FromMilliseconds(100);

            _screenHelper = screenHelper;
        }

        #region Public Methods

        public Color[] GetColors()
        {
            var colors = _screenHelper.GetEmptyColorList();
            for (var i = 0; i < _screenHelper.VerticalLedCount * 2 + _screenHelper.HorizontalLedCount; i++)
            {
                colors.Add(Color);
            }
            return colors.ToArray();
        }

        #endregion 
    }
}