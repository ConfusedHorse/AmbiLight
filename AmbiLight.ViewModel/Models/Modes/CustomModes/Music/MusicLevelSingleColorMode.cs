using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows.Media.Imaging;
using AmbiLight.CrossCutting.Helpers;
using AmbiLight.CrossCutting.Module;
using AmbiLight.ViewModel.Models.Modes.CustomModes.Miscellaneous;
using AmbiLight.ViewModel.Module;
using AmbiLight.ViewModel.Properties;
using GalaSoft.MvvmLight;
using NAudio.CoreAudioApi;
using Ninject;

namespace AmbiLight.ViewModel.Models.Modes.CustomModes.Music
{
    public class MusicLevelSingleColorMode : ViewModelBase, IAmbiLightMode
    {
        #region Fields

        private readonly ScreenHelper _screenHelper;
        private readonly IAmbiLightMode _singleColorMode;
        private bool _isActive;
        private MMDevice _defaultOutputDevice;

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

        #region Peaks

        internal int PercentualLeftPeak { get; set; }

        internal int PercentualRightPeak { get; set; }

        #endregion

        #endregion
        
        public MusicLevelSingleColorMode(ScreenHelper screenHelper, SingleColorMode singleColorMode)
        {
            Name = Resources.MusicLevelSingleColorName;
            ToolTip = Resources.MusicLevelSingleColorToolTip;
            ImageSource = "bucket64.png".ToBitmapFrom(Local.ImagePath);
            GroupName = "Music";
            HasColorPicker = true;
            IsModeGroup = false;

            RecommendedInterval = TimeSpan.FromMilliseconds(50);

            _screenHelper = screenHelper;
            _singleColorMode = singleColorMode;
            _defaultOutputDevice = AudioAccessHelper.GetDefaultOutputDevice(Role.Multimedia);
            AmbiLightKernel.Instance.Get<AudioEventHelper>().DefaultDeviceChanged += OnDefaultDeviceChanged;
        }

        private void OnDefaultDeviceChanged(object sender, DefaultDeviceChangedEventArgs defaultDeviceChangedEventArgs)
        {
            var device = sender as MMDevice;
            if (device != null)
            {
                _defaultOutputDevice =
                    AudioAccessHelper.GetActiveOutoutDevices()
                        .FirstOrDefault(mmd => mmd.FriendlyName == device.FriendlyName);
            }
        }

        private void GetPeakInformation()
        {
            var leftPeak = _defaultOutputDevice.AudioMeterInformation.PeakValues[0] * 100;
            var rightPeak = 0d;
            if (_defaultOutputDevice.AudioMeterInformation.PeakValues.Count > 1)
                rightPeak = _defaultOutputDevice.AudioMeterInformation.PeakValues[1] * 100;

            PercentualLeftPeak = (int)leftPeak;
            PercentualRightPeak = (int)rightPeak;
        }

        #region Public Methods

        public Color[] GetColors()
        {
            if (_defaultOutputDevice == null)
            {
                _defaultOutputDevice = AudioAccessHelper.GetDefaultOutputDevice(Role.Multimedia);
                return _screenHelper.GetEmptyColorArray();
            }

            GetPeakInformation();
            return
                _singleColorMode.GetColors().DimColors((byte)((PercentualLeftPeak + PercentualRightPeak) * 0.5 * 2.55));
        }

        #endregion 
    }
}