using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using AmbiLight.CrossCutting.Helpers;
using AmbiLight.CrossCutting.Module;
using AmbiLight.ViewModel.Models.Modes;
using AmbiLight.ViewModel.Properties;
using GalaSoft.MvvmLight;
using Ninject;
using Color = System.Drawing.Color;

namespace AmbiLight.ViewModel.Models
{
    public class AmbiLightViewModel : ViewModelBase
    {
        #region Fields

        private readonly DispatcherTimer _ambiLightDispatcherTimer = new DispatcherTimer();
        private readonly DispatcherTimer _previewDispatcherTimer = new DispatcherTimer();
        private readonly TimeSpan _defaultTimeSpan = TimeSpan.FromMilliseconds(1000d / Settings.Default.Frames);
        private readonly TimeSpan _previewTimeSpan = TimeSpan.FromMilliseconds(200d);
        private readonly CommunicationHelper _usbConnection;
        private readonly ScreenHelper _screenHelper;
        
        private ObservableCollection<IAmbiLightMode> _availlableModes = new ObservableCollection<IAmbiLightMode>();
        private IAmbiLightMode _currentMode;
        private Color _selectedColor;
        private readonly bool _initialized;
        private readonly byte _dim;
        private bool _preview;

        #endregion

        public AmbiLightViewModel(CommunicationHelper communicationHelper, ScreenHelper screenHelper)
        {
            _screenHelper = screenHelper;
            _screenHelper.HorizontalLedCount = Settings.Default.HorizontalLedCount;
            _screenHelper.VerticalLedCount = Settings.Default.VerticalLedCount;
            _screenHelper.Merge = Settings.Default.Merge;

            _usbConnection = communicationHelper;
            _usbConnection.FindArduino();

            LoadCustomModes();
            LoadExtensionModes();

            _dim = Settings.Default.Dim;
            SelectedColor = Settings.Default.Color;

            _ambiLightDispatcherTimer.Interval = _defaultTimeSpan;
            _ambiLightDispatcherTimer.Tick += AmbiLightDispatcherTimerOnTick;
            _ambiLightDispatcherTimer.Start();

            _previewDispatcherTimer.Interval = _previewTimeSpan;
            _previewDispatcherTimer.Tick += PreviewDispatcherTimerOnTick;

            PostEmpty();
            _initialized = true;
        }

        private void AmbiLightDispatcherTimerOnTick(object sender, EventArgs eventArgs)
        {
            if (!_usbConnection.Connected)
            {
                _usbConnection.FindArduino();
            }
            else
            {
                if (_currentMode == null || _preview) return;
                var colors = _currentMode.GetColors();
                _usbConnection.SendDataToPort(colors.ToColorString(_dim));
            }
        }

        private void PreviewDispatcherTimerOnTick(object sender, EventArgs eventArgs)
        {
            _previewDispatcherTimer.Stop();
            _preview = false;
        }

        #region Properties

        public IAmbiLightMode CurrentMode
        {
            get { return _currentMode; }
            set
            {
                if (value.IsModeGroup)
                {
                    //change subMode instead
                    var lastActiveMode = value.SubModes.FirstOrDefault(sm => sm.WasLastActive);
                    CurrentMode = lastActiveMode ?? value.SubModes.First();
                }
                else
                {
                    //apply IsActive
                    if (_currentMode != null)
                    {
                        _currentMode.IsActive = false;
                        var modeGroup =
                            _availlableModes.FirstOrDefault(
                                am => am.IsModeGroup && am.GroupName == _currentMode.GroupName);
                        if (modeGroup != null) modeGroup.IsActive = false;
                    }

                    //deactivate on second click
                    if (_currentMode == value)
                    {
                        //post empty
                        Task.Run(() =>
                        {
                            //await running colors
                            Thread.Sleep(50);
                            PostEmpty();
                        });

                        //deactivate on second click
                        _currentMode = null;
                        return;
                    }

                    _currentMode = value;

                    //apply IsActive
                    if (_currentMode != null)
                    {
                        _currentMode.IsActive = true;
                        var modeGroup =
                            _availlableModes.FirstOrDefault(
                                am => am.IsModeGroup && am.GroupName == _currentMode.GroupName);
                        if (modeGroup != null)
                        {
                            modeGroup.IsActive = true;
                            foreach (var subMode in modeGroup.SubModes)
                            {
                                subMode.WasLastActive = subMode.Name == _currentMode.Name;
                            }
                        }

                        //apply recommended interval
                        _ambiLightDispatcherTimer.Interval = _currentMode.RecommendedInterval != default(TimeSpan)
                            ? _currentMode.RecommendedInterval
                            : _defaultTimeSpan;
                    }
                }
            }
        }

        public ObservableCollection<IAmbiLightMode> AvaillableModes
        {
            get { return _availlableModes; }
            set
            {
                _availlableModes = value;
                RaisePropertyChanged();
            }
        }

        public Color SelectedColor
        {
            get { return _selectedColor; }
            set
            {
                _selectedColor = value;
                if (_initialized) PreviewSelection(value);

                foreach (var ambiLightMode in _availlableModes)
                {
                    if (ambiLightMode.IsModeGroup)
                    foreach (var ambiLightSubMode in ambiLightMode.SubModes.Where(sm => sm.HasColorPicker))
                    {
                            ambiLightSubMode.Color = _selectedColor;
                    }
                    else
                    {
                        ambiLightMode.Color = _selectedColor;
                    }
                }

                Settings.Default.Color = _selectedColor;
            }
        }

        #endregion

        #region Private Methods

        private void LoadCustomModes()
        {
            var customModes = AmbiLightKernel.Instance.GetAll<IAmbiLightMode>();
            var groupedModes = customModes
                .OrderByDescending(cm => cm.SubModes?.Count)
                .GroupBy(cm => cm.GroupName);

            foreach (var groupedMode in groupedModes)
            {
                if (groupedMode.Count() == 1)
                {
                    // add mode without group
                    _availlableModes.Add(groupedMode.First());
                }
                else
                {
                    // find ModeGroup
                    var modeGroup = groupedMode.FirstOrDefault(gm => gm.IsModeGroup);
                    if (modeGroup == null)
                    {
                        // add all modes without group header
                        foreach (var customMode in groupedMode)
                        {
                            _availlableModes.Add(customMode);
                        }
                    }
                    else
                    {
                        // add mode group
                        _availlableModes.Add(modeGroup);
                    }
                }
            }
        }

        private void LoadExtensionModes()
        {
            //TODO implement loading from assembly directory
        }

        private void PreviewSelection(Color previewColor)
        {
            _preview = true;
            _previewDispatcherTimer.Stop();
            _previewDispatcherTimer.Start();

            var colors = _screenHelper.GetEmptyColorList();
            for (var i = 0; i < _screenHelper.VerticalLedCount * 2 + _screenHelper.HorizontalLedCount; i++)
            {
                colors.Add(previewColor);
            }
            var colorArray = colors.ToArray();
            _usbConnection.SendDataToPort(colorArray.ToColorString(_dim));
        }

        internal void PostEmpty()
        {
            var blackString = string.Empty;
            for (var i = 0; i < _screenHelper.VerticalLedCount * 2 + _screenHelper.HorizontalLedCount; i++)
            {
                blackString += $"{(char) 1}{(char) 1}{(char) 1}";
            }
            _usbConnection.SendDataToPort(blackString);
        }

        internal void SaveSettings()
        {
            Settings.Default.Save();
        }

        #endregion
    }
}