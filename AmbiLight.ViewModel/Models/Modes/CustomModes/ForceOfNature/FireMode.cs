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
    public class FireMode : ViewModelBase, IAmbiLightMode
    {
        #region Fields

        private bool _isActive;
        
        private readonly Color[] _lastColors;
        private readonly Color[] _firePalette;
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

        public FireMode(ScreenHelper screenHelper)
        {
            Name = Resources.FireName;
            ToolTip = Resources.FireToolTip;
            ImageSource = "fire64.png".ToBitmapFrom(Local.ImagePath);
            GroupName = "ForceOfNature";
            HasColorPicker = false;
            IsModeGroup = false;

            RecommendedInterval = TimeSpan.FromMilliseconds(150);

            _firePalette = new[]
            {
                Color.FromArgb(255, 000, 000),
                Color.FromArgb(253, 068, 003),
                Color.FromArgb(253, 115, 003),
                Color.FromArgb(243, 215, 005)
            };

            _lastColors = screenHelper.GetEmptyColorArray();
            for (var i = 0; i < _lastColors.Length; i++)
            {
                _lastColors[i] = _firePalette[_random.Next(0, _firePalette.Length)];
            }
        }

        #region Public Methods

        public Color[] GetColors()
        {
            for (var i = 0; i < _lastColors.Length; i++)
            {
                _lastColors[i] = _lastColors[i].Merge(_firePalette[_random.Next(0, _firePalette.Length)]);
            }
            return _lastColors.ToArray();
        }

        #endregion 
    }
}