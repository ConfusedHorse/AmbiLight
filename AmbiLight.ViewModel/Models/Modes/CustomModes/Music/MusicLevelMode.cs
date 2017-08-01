using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows.Media.Imaging;
using AmbiLight.CrossCutting.Helpers;
using AmbiLight.CrossCutting.Module;
using AmbiLight.ViewModel.Module;
using AmbiLight.ViewModel.Properties;
using GalaSoft.MvvmLight;
using NAudio.CoreAudioApi;
using Ninject;

namespace AmbiLight.ViewModel.Models.Modes.CustomModes.Music
{
    public class MusicLevelMode : ViewModelBase, IAmbiLightMode
    {
        #region Fields

        private readonly Color[] _levelColors;
        private const double DelayLevelDecay = 5d;
        private readonly ScreenHelper _screenHelper;
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

        internal double PercentualLeftPeak { get; set; }

        internal double PercentualLeftPeakDelay { get; set; }

        internal double PercentualRightPeak { get; set; }

        internal double PercentualRightPeakDelay { get; set; }

        #endregion

        #endregion

        public MusicLevelMode(ScreenHelper screenHelper)
        {
            Name = Resources.MusicLevelName;
            ToolTip = Resources.MusicLevelToolTip;
            ImageSource = "bars64.png".ToBitmapFrom(Local.ImagePath);
            GroupName = "Music";
            HasColorPicker = false;
            IsModeGroup = false;

            RecommendedInterval = TimeSpan.FromMilliseconds(50);

            _screenHelper = screenHelper;
            _defaultOutputDevice = AudioAccessHelper.GetDefaultOutputDevice(Role.Multimedia);
            AmbiLightKernel.Instance.Get<AudioEventHelper>().DefaultDeviceChanged += OnDefaultDeviceChanged;

            var levelPalette = new[]
            {
                Color.FromArgb(000, 255, 000),
                Color.FromArgb(000, 000, 255)
            };

            _levelColors = screenHelper.GetEmptyColorArray();
            for (var i = 0; i < _levelColors.Length; i++)
            {
                var weight = 2d * Math.Abs(i - 20) / _levelColors.Length;
                _levelColors[i] = levelPalette.First().Merge(levelPalette.Last(), weight);
            }
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
            var samplesAccumulated = 0d;
            var endAt = DateTime.Now.AddMilliseconds(RecommendedInterval.TotalMilliseconds / 25);

            var leftPeak = 0d;
            var rightPeak = 0d;

            var stereo = _defaultOutputDevice.AudioMeterInformation.PeakValues.Count > 1;
            while (DateTime.Now < endAt)
            {
                samplesAccumulated++;
                leftPeak += _defaultOutputDevice.AudioMeterInformation.PeakValues[0];
                if (stereo) rightPeak += _defaultOutputDevice.AudioMeterInformation.PeakValues[1];
            }

            leftPeak *= 100 / samplesAccumulated;
            if (!stereo) rightPeak = leftPeak;
            else rightPeak *= 100 / samplesAccumulated;

            if (leftPeak < PercentualLeftPeakDelay) PercentualLeftPeakDelay -= DelayLevelDecay;
            else PercentualLeftPeakDelay = leftPeak;
            if (rightPeak < PercentualRightPeakDelay) PercentualRightPeakDelay -= DelayLevelDecay;
            else PercentualRightPeakDelay = rightPeak;

            PercentualLeftPeak = leftPeak;
            PercentualRightPeak = rightPeak;
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

            var levelColors = _screenHelper.GetEmptyColorArray();
            var levelImpact = levelColors.Length / 2d;
            var leftLevelStrongLeds = PercentualLeftPeak / 100d * levelImpact;
            var leftLevelWeakLeds = PercentualLeftPeakDelay / 100d * levelImpact;
            var rightLevelStrongLeds = PercentualRightPeak / 100d * levelImpact;
            var rightLevelWeakLeds = PercentualRightPeakDelay / 100d * levelImpact;

            for (var i = 0; i < levelImpact; i++)
            {
                var ii = levelColors.Length - i - 1;

                //left levels
                if (leftLevelStrongLeds > i)
                {
                    var strongImpact = leftLevelStrongLeds - Math.Truncate(leftLevelStrongLeds);
                    levelColors[ii] = _levelColors[ii].DimColor(strongImpact);

                    if (leftLevelWeakLeds > i)
                    {
                        var weakImpact = leftLevelWeakLeds - Math.Truncate(leftLevelWeakLeds);
                        levelColors[ii] = _levelColors[ii].DimColor(weakImpact).Merge(levelColors[ii], 0.5);
                    }
                }

                //right levels
                if (rightLevelStrongLeds > i)
                {
                    var strongImpact = rightLevelStrongLeds - Math.Truncate(rightLevelStrongLeds);
                    levelColors[i] = _levelColors[i].DimColor(strongImpact);

                    if (rightLevelWeakLeds > i)
                    {
                        var weakImpact = rightLevelWeakLeds - Math.Truncate(rightLevelWeakLeds);
                        levelColors[i] = _levelColors[i].DimColor(weakImpact).Merge(levelColors[i], 0.5);
                    }
                }
            }

            return levelColors;
        }

        #endregion 
    }
}