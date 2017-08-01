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
    public class FlashMode : ViewModelBase, IAmbiLightMode
    {
        #region Fields

        private bool _isActive;
        
        private readonly Color[] _lastColors;
        private readonly Color[] _waterPalette;
        private readonly Random _random = new Random();

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

        public FlashMode(ScreenHelper screenHelper)
        {
            Name = Resources.FlashName;
            ToolTip = Resources.FlashToolTip;
            ImageSource = "flash64.png".ToBitmapFrom(Local.ImagePath);
            GroupName = "ForceOfNature";
            HasColorPicker = false;
            IsModeGroup = false;

            RecommendedInterval = TimeSpan.FromMilliseconds(50);

            _waterPalette = new[]
            {
                Color.FromArgb(84, 29, 135),
                Color.FromArgb(188, 188, 24),
                Color.FromArgb(60, 156, 205),
                Color.FromArgb(0, 0, 0), Color.FromArgb(0, 0, 0), Color.FromArgb(0, 0, 0),
                Color.FromArgb(0, 0, 0), Color.FromArgb(0, 0, 0), Color.FromArgb(0, 0, 0),
                Color.FromArgb(0, 0, 0), Color.FromArgb(0, 0, 0), Color.FromArgb(0, 0, 0),
                Color.FromArgb(0, 0, 0), Color.FromArgb(0, 0, 0), Color.FromArgb(0, 0, 0),
                Color.FromArgb(0, 0, 0), Color.FromArgb(0, 0, 0), Color.FromArgb(0, 0, 0),
            };

            _lastColors = screenHelper.GetEmptyColorArray();
            for (var i = 0; i < _lastColors.Length; i++)
            {
                _lastColors[i] = _waterPalette[_random.Next(0, _waterPalette.Length)];
            }
        }

        #region Public Methods

        public Color[] GetColors()
        {
            for (var i = 0; i < _lastColors.Length; i++)
            {
                _lastColors[i] = _lastColors[i].Merge(_waterPalette[_random.Next(0, _waterPalette.Length)]);
            }
            return _lastColors.ToArray();
        }

        #endregion 
    }
}