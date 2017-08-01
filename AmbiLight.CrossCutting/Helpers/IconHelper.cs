using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;

namespace AmbiLight.CrossCutting.Helpers
{
    public static class IconHelper
    {
        #region Icon mapping

        public static Image ToImageFrom(this string identifier, string packagePrefix)
        {
            return new Image
            {
                Source = identifier.ToBitmapFrom(packagePrefix)
            };
        }

        public static BitmapImage ToBitmapFrom(this string identifier, string packagePrefix)
        {
            return $"{packagePrefix}{identifier}".ToUri().ToBitmap();
        }

        private static BitmapImage ToBitmap(this Uri uri)
        {
            return new BitmapImage(uri);
        }

        public static Icon ToIconFrom(this string identifier, string packagePrefix)
        {
            return $"{packagePrefix}{identifier}".ToUri().ToIcon();
        }

        private static Icon ToIcon(this Uri uri)
        {
            var iconStream = Application.GetResourceStream(uri);
            return iconStream == null ? null : new Icon(iconStream.Stream);
        }

        private static Uri ToUri(this string uri)
        {
            return new Uri(uri);
        }

        #endregion
    }
}