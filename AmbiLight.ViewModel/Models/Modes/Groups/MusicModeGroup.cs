using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Media.Imaging;
using AmbiLight.CrossCutting.Helpers;
using AmbiLight.ViewModel.Models.Modes.CustomModes.Music;
using AmbiLight.ViewModel.Module;
using GalaSoft.MvvmLight;

namespace AmbiLight.ViewModel.Models.Modes.Groups
{
    public class MusicModeGroup : ViewModelBase, IAmbiLightMode
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

        public MusicModeGroup(
            MusicLevelSingleColorMode musicLevelSingleColorMode, 
            MusicLevelRainbowMode musicLevelRainbowMode,
            MusicLevelMode musicLevelMode)
        {
            GroupName = "Music";
            ImageSource = "music64.png".ToBitmapFrom(Local.ImagePath);
            HasColorPicker = false;
            IsModeGroup = true;

            SubModes.Add(musicLevelSingleColorMode);
            SubModes.Add(musicLevelRainbowMode);
            SubModes.Add(musicLevelMode);
        }

        #region Public Methods

        public Color[] GetColors()
        {
            return null;
        }

        #endregion 
    }
}