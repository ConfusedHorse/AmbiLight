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
    public class LeafMode : ViewModelBase, IAmbiLightMode
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

        public LeafMode(ScreenHelper screenHelper)
        {
            Name = Resources.LeafName;
            ToolTip = Resources.LeafToolTip;
            ImageSource = "leaf64.png".ToBitmapFrom(Local.ImagePath);
            GroupName = "ForceOfNature";
            HasColorPicker = false;
            IsModeGroup = false;

            RecommendedInterval = TimeSpan.FromMilliseconds(300);

            _waterPalette = new[]
            {
                Color.FromArgb(000, 255, 000),
                Color.FromArgb(123, 255, 000),
                Color.FromArgb(123, 200, 000),
                Color.FromArgb(000, 200, 173)
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
                _lastColors[i] = _lastColors[i].Merge(_waterPalette[_random.Next(0, _waterPalette.Length)], 0.25);
            }
            return _lastColors.ToArray();
        }

        #endregion 
    }
}