using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Media.Imaging;
using AmbiLight.CrossCutting.Helpers;
using AmbiLight.ViewModel.Models.Modes.CustomModes.ForceOfNature;
using AmbiLight.ViewModel.Module;
using GalaSoft.MvvmLight;

namespace AmbiLight.ViewModel.Models.Modes.Groups
{
    public class ForceOfNatureModeGroup : ViewModelBase, IAmbiLightMode
    {
        #region Fields
        
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

        public ObservableCollection<IAmbiLightMode> SubModes { get; } = new ObservableCollection<IAmbiLightMode>();

        #endregion

        public ForceOfNatureModeGroup(
            RainbowMode rainbowMode, 
            FireMode fireMode,
            WaterMode waterMode,
            EarthMode earthMode,
            LeafMode leafMode,
            FlashMode flashMode)
        {
            GroupName = "ForceOfNature";
            ImageSource = "planet64.png".ToBitmapFrom(Local.ImagePath);
            HasColorPicker = false;
            IsModeGroup = true;

            SubModes.Add(rainbowMode);
            SubModes.Add(fireMode);
            SubModes.Add(waterMode);
            //SubModes.Add(earthMode);
            SubModes.Add(leafMode);
            //SubModes.Add(flashMode);
        }

        #region Public Methods

        public Color[] GetColors()
        {
            return null;
        }

        #endregion 
    }
}