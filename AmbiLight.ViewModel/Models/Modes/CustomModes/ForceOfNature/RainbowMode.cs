using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows.Media.Imaging;
using AmbiLight.CrossCutting.Helpers;
using AmbiLight.ViewModel.Module;
using AmbiLight.ViewModel.Properties;
using GalaSoft.MvvmLight;

namespace AmbiLight.ViewModel.Models.Modes.CustomModes.ForceOfNature
{
    public class RainbowMode : ViewModelBase, IAmbiLightMode
    {
        #region Fields

        private const int StepSize = 30;

        private readonly ScreenHelper _screenHelper;
        private bool _isActive;

        private Color[] _lastColors;

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

        public RainbowMode(ScreenHelper screenHelper)
        {
            Name = Resources.RainbowName;
            ToolTip = Resources.RainbowToolTip;
            ImageSource = "rainbow64.png".ToBitmapFrom(Local.ImagePath);
            GroupName = "ForceOfNature";
            HasColorPicker = false;
            IsModeGroup = false;

            RecommendedInterval = TimeSpan.FromMilliseconds(50);

            _screenHelper = screenHelper;
            InitializeRainbowColors();
        }

        private void InitializeRainbowColors()
        {
            var lastColor = Color.Red;
            _lastColors = _screenHelper.GetEmptyColorArray();

            for (var i = 0; i < _lastColors.Length; i++)
            {
                lastColor = lastColor.GetNextColor(StepSize);
                _lastColors[i] = lastColor;
            }
        }

        #region Public Methods

        public Color[] GetColors()
        {
            var lastFirstColor = _lastColors[0];
            _lastColors.PushColors();
            _lastColors[0] = lastFirstColor.GetNextColor(StepSize);
            return _lastColors.ToArray();
        }

        #endregion 
    }
}