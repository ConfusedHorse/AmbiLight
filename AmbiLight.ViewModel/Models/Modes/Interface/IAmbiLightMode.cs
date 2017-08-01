using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace AmbiLight.ViewModel.Models.Modes
{
    public interface IAmbiLightMode
    {
        /// <summary>
        /// mode name for displaying and selecting purposes, must be unique!
        /// </summary>
        string Name { get; }

        /// <summary>
        /// mode tooltip for displaying purposes
        /// </summary>
        string ToolTip { get; }

        /// <summary>
        /// shows if the mode or a mode of the groupd is currently running
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        /// if this is set true there will be a mouse over triggered color picker
        /// </summary>
        bool HasColorPicker { get; }

        /// <summary>
        /// if this is set true the mode will be a group
        /// </summary>
        bool IsModeGroup { get; }

        /// <summary>
        /// determines if this was the last active Mode in a Group
        /// </summary>
        bool WasLastActive { get; set; }

        /// <summary>
        /// setting group dependency if a group is defined
        /// </summary>
        string GroupName { get; }

        /// <summary>
        /// an image for displaying purposes
        /// </summary>
        BitmapImage ImageSource { get; }

        /// <summary>
        /// setting the interval for this mode, leave blank for default interval 
        /// </summary>
        TimeSpan RecommendedInterval { get; }

        /// <summary>
        /// color used by this mode in case it uses a chosen color
        /// </summary>
        Color Color { get; set; }

        /// <summary>
        /// function called to acquire color array for LEDs
        /// </summary>
        /// <returns>a color array</returns>
        Color[] GetColors();

        /// <summary>
        /// used to define mode groups, leave blank if this is a mode
        /// </summary>
        ObservableCollection<IAmbiLightMode> SubModes { get; }
    }
}