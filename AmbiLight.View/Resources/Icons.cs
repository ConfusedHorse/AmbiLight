using System.Drawing;
using System.Windows.Media.Imaging;
using AmbiLight.CrossCutting.Helpers;
using Image = System.Windows.Controls.Image;

namespace AmbiLight.View.Resources
{
    public static class Icons
    {
        private const string PackagePrefix = "pack://application:,,,/AmbiLight.View;component/Resources/Icons/";

        public static Icon Rgb16 => "rgb16.ico".ToIconFrom(PackagePrefix);

        public static Image Close16 => "close16.png".ToImageFrom(PackagePrefix);

        public static BitmapImage ReferenceColors => "rgbMatrixActualColors.png".ToBitmapFrom(PackagePrefix);
    }
}