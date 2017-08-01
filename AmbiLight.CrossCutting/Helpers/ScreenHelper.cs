using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using AmbiLight.Enums;
using Size = System.Drawing.Size;
using Point = System.Drawing.Point;

namespace AmbiLight.CrossCutting.Helpers
{
    public class ScreenHelper
    {
        #region Fields

        private const int ColorBytes = 4;
        private const double AdjustmentRatio = 76.8;

        #endregion Fields

        #region Properties

        internal Size Resolution { get; set; }

        public int VerticalLedCount { get; set; } = 1;

        public int HorizontalLedCount { get; set; } = 1;

        public int Offset { get; set; }

        public int Merge { get; set; }

        #endregion Properties

        public ScreenHelper()
        {
            Resolution = GetElementPixelSize();
            Offset = (int)(Resolution.Width / AdjustmentRatio);
            //Merge = (int)(Resolution.Width / AdjustmentRatio);
            SystemParameters.StaticPropertyChanged += SystemParametersOnStaticPropertyChanged;
        }

        private void SystemParametersOnStaticPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName != "PrimaryScreenWidth" &&
                propertyChangedEventArgs.PropertyName != "PrimaryScreenHeight") return;
            Resolution = GetElementPixelSize();
        }

        #region Internal Methods

        public Color[] GetEmptyColorArray()
        {
            return new Color[2 * VerticalLedCount + HorizontalLedCount];
        }

        public List<Color> GetEmptyColorList()
        {
            return new List<Color>();
        }

        internal static Size GetElementPixelSize()
        {
            using (var graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                var pixelWidth = (int)(SystemParameters.PrimaryScreenWidth * graphics.DpiX / 96.0);
                var pixelHeight = (int)(SystemParameters.PrimaryScreenHeight * graphics.DpiY / 96.0);
                Debug.WriteLine($"Width: {pixelWidth}, Height: {pixelHeight}");
                return new Size(pixelWidth, pixelHeight);
            }
        }

        public Color[] CaptureColorArray(Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.Left:
                    return LeftColorArray();
                case Orientation.Top:
                    return TopColorArray();
                case Orientation.Right:
                    return RightColorArray();
                case Orientation.Bottom:
                    return BottomColorArray();
                default:
                    return null;
            }
        }

        #endregion Internal Methods

        #region Private Methods

        #region Helper

        private static byte[] GetByteArrayFromBitmap(Bitmap bitmap)
        {
            var dimensions = new Rectangle(new Point(0, 0), bitmap.Size);
            var bits = bitmap.LockBits(dimensions, ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var leftIntPtr = bits.Scan0;
            var byteCount = Math.Abs(bits.Stride) * dimensions.Height;
            var bytes = new byte[byteCount];

            Marshal.Copy(leftIntPtr, bytes, 0, byteCount);

            bitmap.UnlockBits(bits);
            return bytes;
        }

        private static byte[][] SplitByteArray(byte[] bytes, int splitCount, int size)
        {
            var byteArrays = new List<byte[]>();
            for (var split = 0; split < splitCount; split++)
            {
                var byteArray = new byte[size];
                Array.Copy(bytes, split * size, byteArray, 0, size);
                byteArrays.Add(byteArray);
            }
            return byteArrays.ToArray();
        }

        private static Color MergeBytesToColor(IReadOnlyList<byte> byteArray, int size)
        {
            var r = 0;
            var g = 0;
            var b = 0;
            for (var byteIndex = 0; byteIndex < size - ColorBytes; byteIndex += ColorBytes)
            {
                b += byteArray[byteIndex];
                g += byteArray[byteIndex + 1];
                r += byteArray[byteIndex + 2];
            }

            var dr = r / (size / ColorBytes);
            var dg = g / (size / ColorBytes);
            var db = b / (size / ColorBytes);
            return Color.FromArgb(dr, dg, db);
        }

        private static Color[] ExtractColorsFromBitmap(Bitmap bitmap, int count, int merge)
        {
            var colors = new Color[count];

            var bytes = GetByteArrayFromBitmap(bitmap);
            var chunkHeight = bitmap.Height / count;
            var chunkByteSize = merge * chunkHeight * ColorBytes;
            var byteArrays = SplitByteArray(bytes, count, chunkByteSize);

            Parallel.For(0, byteArrays.Length, ledIndex =>
            {
                var color = MergeBytesToColor(byteArrays[ledIndex], chunkByteSize);
                colors[ledIndex] = color;
            });
            return colors;
        }

        #endregion Helper

        private Color[] LeftColorArray()
        {
            var bitmap = new Bitmap(Merge, Resolution.Height - 2 * Offset);

            try
            {
                Graphics.FromImage(bitmap)
                    .CopyFromScreen(Offset, Offset, 0, 0, bitmap.Size, CopyPixelOperation.SourceCopy);
            }
            catch (Exception)
            {
                return new Color[VerticalLedCount];
            }

            return ExtractColorsFromBitmap(bitmap, VerticalLedCount, Merge);
        }

        private Color[] TopColorArray()
        {
            var bitmap = new Bitmap(Resolution.Width - 2 * Offset, Merge);

            try
            {
                Graphics.FromImage(bitmap)
                    .CopyFromScreen(Offset, Offset, 0, 0, bitmap.Size, CopyPixelOperation.SourceCopy);
                bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
            }
            catch (Exception)
            {
                return new Color[HorizontalLedCount];
            }

            return ExtractColorsFromBitmap(bitmap, HorizontalLedCount, Merge);
        }

        private Color[] RightColorArray()
        {
            var bitmap = new Bitmap(Merge, Resolution.Height - 2 * Offset);

            try
            {
                Graphics.FromImage(bitmap)
                    .CopyFromScreen(Resolution.Width - (Offset + Merge), Offset, 0, 0, bitmap.Size, CopyPixelOperation.SourceCopy);
                bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
            }
            catch (Exception)
            {
                return new Color[VerticalLedCount];
            }

            return ExtractColorsFromBitmap(bitmap, VerticalLedCount, Merge);
        }

        private Color[] BottomColorArray()
        {
            var bitmap = new Bitmap(Resolution.Width - 2 * Offset, Merge);

            try
            {
                Graphics.FromImage(bitmap)
                    .CopyFromScreen(Offset, Resolution.Height - (Offset + Merge), 0, 0, bitmap.Size, CopyPixelOperation.SourceCopy);
                bitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
            }
            catch (Exception)
            {
                return new Color[HorizontalLedCount];
            }

            return ExtractColorsFromBitmap(bitmap, HorizontalLedCount, Merge);
        }

        #endregion Private Methods
    }
}