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
    public class EarthMode : ViewModelBase, IAmbiLightMode
    {
        #region Fields

        private bool _isActive;
        
        private readonly Color[] _lastColors;
        private readonly Color[] _earthPalette;
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

        public EarthMode(ScreenHelper screenHelper)
        {
            Name = Resources.EarthName;
            ToolTip = Resources.EarthToolTip;
            ImageSource = "earth64.png".ToBitmapFrom(Local.ImagePath);
            GroupName = "ForceOfNature";
            HasColorPicker = false;
            IsModeGroup = false;

            RecommendedInterval = TimeSpan.FromMilliseconds(300);

            _earthPalette = new[]
            {
                Color.FromArgb(114, 096, 027),
                Color.FromArgb(089, 058, 014),
                Color.FromArgb(100, 058, 018),
                Color.FromArgb(075, 033, 018)
            };

            _lastColors = screenHelper.GetEmptyColorArray();
            for (var i = 0; i < _lastColors.Length; i++)
            {
                _lastColors[i] = _earthPalette[_random.Next(0, _earthPalette.Length)];
            }
        }

        #region Public Methods

        public Color[] GetColors()
        {
            for (var i = 0; i < _lastColors.Length; i++)
            {
                _lastColors[i] = _lastColors[i].Merge(_earthPalette[_random.Next(0, _earthPalette.Length)], 0.66);
            }
            return _lastColors.ToArray();
        }

        #endregion 
    }
}