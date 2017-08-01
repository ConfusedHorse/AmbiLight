using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Media.Imaging;
using AmbiLight.CrossCutting.Helpers;
using AmbiLight.ViewModel.Models.Modes.CustomModes.Miscellaneous;
using AmbiLight.ViewModel.Module;
using GalaSoft.MvvmLight;

namespace AmbiLight.ViewModel.Models.Modes.Groups
{
    public class MiscellaneousModeGroup : ViewModelBase, IAmbiLightMode
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

        public MiscellaneousModeGroup(
            SingleColorMode singleColorMode,
            AmbiLightMode ambiLightMode,
            AmbiLightSaturatedMode ambiLightSaturatedMode)
        {
            GroupName = "Miscellaneous";
            ImageSource = "miscellaneous64.png".ToBitmapFrom(Local.ImagePath);
            HasColorPicker = false;
            IsModeGroup = true;

            SubModes.Add(ambiLightMode);
            //SubModes.Add(ambiLightSaturatedMode);
            SubModes.Add(singleColorMode);
        }

        #region Public Methods

        public Color[] GetColors()
        {
            return null;
        }

        #endregion 
    }
}