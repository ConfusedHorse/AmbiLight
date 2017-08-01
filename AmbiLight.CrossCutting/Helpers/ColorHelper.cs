using System;
using System.Drawing;
using System.Text;

namespace AmbiLight.CrossCutting.Helpers
{
    public static class ColorHelper
    {
        public static string ToColorString(this Color[] colorArray, byte dim = 255)
        {
            colorArray.DimColors(dim).ExcludeZeros();

            var sb = new StringBuilder();
            foreach (var color in colorArray)
                sb.Append($"{(char) color.B}{(char) color.G}{(char) color.R}");
            return sb.ToString();
        }

        public static Color[] ExcludeZeros(this Color[] colorArray)
        {
            for (var index = 0; index < colorArray.Length; index++)
            {
                var color = colorArray[index];
                var r = color.R == 0 ? 1 : color.R;
                var g = color.G == 0 ? 1 : color.G;
                var b = color.B == 0 ? 1 : color.B;
                colorArray[index] = Color.FromArgb(0, r, g, b);
            }
            return colorArray;
        }

        public static Color[] DimColors(this Color[] colorArray, byte dim)
        {
            for (var index = 0; index < colorArray.Length; index++)
            {
                var color = colorArray[index];
                var r = (int) (color.R / 255d * dim);
                var g = (int) (color.G / 255d * dim);
                var b = (int) (color.B / 255d * dim);
                colorArray[index] = Color.FromArgb(0, r, g, b);
            }
            return colorArray;
        }

        public static Color DimColor(this Color color, byte dim)
        {
            var r = (int) (color.R / 255d * dim);
            var g = (int) (color.G / 255d * dim);
            var b = (int) (color.B / 255d * dim);
            return Color.FromArgb(0, r, g, b);
        }

        public static Color DimColor(this Color color, double dim)
        {
            if (dim > 1 || dim < 0) return color;
            var r = (int) (color.R * dim);
            var g = (int) (color.G * dim);
            var b = (int) (color.B * dim);
            return Color.FromArgb(0, r, g, b);
        }

        public static Color[] InvertColors(this Color[] colorArray)
        {
            for (var index = 0; index < colorArray.Length; index++)
            {
                var color = colorArray[index];
                var r = 255 - color.R;
                var g = 255 - color.G;
                var b = 255 - color.B;
                colorArray[index] = Color.FromArgb(0, r, g, b);
            }
            return colorArray;
        }

        public static Color[] PushColors(this Color[] colorArray)
        {
            for (var i = colorArray.Length - 1; i > 0; i--)
            {
                colorArray[i] = colorArray[i - 1];
            }
            colorArray[0] = Color.Black;
            return colorArray;
        }

        public static Color GetNextColor(this Color color, int steps = 15)
        {
            var r = color.R;
            var g = color.G;
            var b = color.B;
            for (var i = 0; i < steps; i++)
            {
                if (r == 255 && g < 255 && b == 0) g += 1;
                if (g == 255 && r > 0 && b == 0) r -= 1;
                if (g == 255 && b < 255 && r == 0) b += 1;
                if (b == 255 && g > 0 && r == 0) g -= 1;
                if (b == 255 && r < 255 && g == 0) r += 1;
                if (r == 255 && b > 0 && g == 0) b -= 1;
            }
            return Color.FromArgb(r, g, b);
        }

        public static Color Merge(this Color color1, Color color2)
        {
            return Color.FromArgb((color1.R + color2.R) / 2, (color1.G + color2.G) / 2, (color1.B + color2.B) / 2);
        }

        public static Color Merge(this Color color1, Color color2, double weight)
        {
            if (weight > 1 || weight < 0) return Merge(color1, color2);
            return Color.FromArgb((int) (color1.R * (1 - weight) + color2.R * weight),
                (int) (color1.G * (1 - weight) + color2.G * weight),
                (int) (color1.B * (1 - weight) + color2.B * weight));
        }

        public static Color[] AlterSaturationBy(this Color[] colorArray, double fraction)
        {
            for (var index = 0; index < colorArray.Length; index++)
            {
                colorArray[index] = colorArray[index].AlterSaturationBy(fraction);
            }
            return colorArray;
        }

        public static Color AlterSaturationBy(this Color color, double fraction)
        {
            if (fraction < -1) fraction = -1;
            else if (fraction > 1) fraction = 1;

            var hslTuple = color.GetHslTuple();
            var h = hslTuple.Item1;
            var s = hslTuple.Item2;
            var l = hslTuple.Item3;

            if (fraction >= 0)
            {
                var grayFactor = s / 255.0;
                var varInterval = 255 - s;
                s = s + fraction * varInterval * grayFactor;
            }
            else
            {
                var varInterval = s;
                s = s + fraction * varInterval;
            }

            hslTuple = new Tuple<double, double, double>(h, s, l);
            return hslTuple.GetRgbColor();
        }

        private static Tuple<double, double, double> GetHslTuple(this Color color)
        {
            var r = color.R / 255d;
            var g = color.G / 255d;
            var b = color.B / 255d;

            double h, s, l;

            var maxColor = Math.Max(r, Math.Max(g, b));
            var minColor = Math.Min(r, Math.Min(g, b));

            if (Math.Abs(minColor - maxColor) < 1d / 255)
            {
                h = 0d;
                s = 0d;
                l = maxColor;
            }
            else
            {
                var dif = maxColor - minColor;
                var sum = maxColor + minColor;
                l = (minColor + maxColor) / 2;
                if (l < 0.5) s = dif / sum;
                else s = dif / (2 - sum);
                if (Math.Abs(r - maxColor) < 1d / 255) h = (g - b) / dif;
                else if (Math.Abs(g - maxColor) < 1d / 255) h = 2.0 + (b - r) / dif;
                else h = 4.0 + (r - g) / dif;
                h /= 6;
                if (h < 0) h++;
            }

            return new Tuple<double, double, double>(h * 360, s * 255, l * 255);
        }

        private static Color GetRgbColor(this Tuple<double, double, double> hslTuple)
        {
            var h = hslTuple.Item1 % 260 / 360;
            var s = hslTuple.Item2 / 256;
            var l = hslTuple.Item3 / 256;

            double r, g, b;
            if (Math.Abs(s) < 1d / 255)
            {
                r = l;
                g = l;
                b = l;
            }
            else
            {
                var temp2 = l < 0.5 ? l * (1 + s) : l + s - l * s;
                var temp1 = 2 * l - temp2;

                var tempr = h + 1.0 / 3.0;
                if (tempr > 1) tempr--;
                var tempg = h;
                var tempb = h - 1.0 / 3.0;
                if (tempb < 0) tempb++;

                //Red
                if (tempr < 1.0 / 6.0) r = temp1 + (temp2 - temp1) * 6.0 * tempr;
                else if (tempr < 0.5) r = temp2;
                else if (tempr < 2.0 / 3.0) r = temp1 + (temp2 - temp1) * (2.0 / 3.0 - tempr) * 6.0;
                else r = temp1;

                //Green
                if (tempg < 1.0 / 6.0) g = temp1 + (temp2 - temp1) * 6.0 * tempg;
                else if (tempg < 0.5) g = temp2;
                else if (tempg < 2.0 / 3.0) g = temp1 + (temp2 - temp1) * (2.0 / 3.0 - tempg) * 6.0;
                else g = temp1;

                //Blue
                if (tempb < 1.0 / 6.0) b = temp1 + (temp2 - temp1) * 6.0 * tempb;
                else if (tempb < 0.5) b = temp2;
                else if (tempb < 2.0 / 3.0) b = temp1 + (temp2 - temp1) * (2.0 / 3.0 - tempb) * 6.0;
                else b = temp1;
            }

            return Color.FromArgb((int) (r * 255.0), (int) (g * 255.0), (int) (b * 255.0));
        }
    }
}