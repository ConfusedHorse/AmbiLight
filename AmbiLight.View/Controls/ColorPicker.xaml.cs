using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using AmbiLight.View.Resources;
using AmbiLight.ViewModel.Models.Events;

namespace AmbiLight.View.Controls
{
    /// <summary>
    /// Interaktionslogik für ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : UserControl
    {
        private bool _isMouseDown;
        private Color _selectedColor;
        private readonly BitmapImage _source = Icons.ReferenceColors;

        private readonly DispatcherTimer _delayDispatcherTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(50)
        };

        public ColorPicker()
        {
            InitializeComponent();

            _delayDispatcherTimer.Tick += DelayDispatcherTimerOnTick;
        }

        private void DelayDispatcherTimerOnTick(object sender, EventArgs eventArgs)
        {
            _delayDispatcherTimer.Stop();
            
            SelectionChanged?.Invoke(this, _selectedColor);
        }

        public event ColorChangedHandler SelectionChanged;
        
        private void RgbMatrixCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = e.GetPosition(RgbMatrixCanvas);
            DrawLineCross(mousePos);

            if (!_isMouseDown) return;

            SetColorFromPosition(mousePos);
            _delayDispatcherTimer.Start();
        }

        private void RgbMatrixCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = true;
        }

        private void RgbMatrixCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isMouseDown = false;

            var mousePos = e.GetPosition(RgbMatrixCanvas);
            SetColorFromPosition(mousePos);
            _delayDispatcherTimer.Start();
        }

        private void RgbMatrixCanvas_OnMouseEnter(object sender, MouseEventArgs e)
        {
            _isMouseDown = Mouse.LeftButton == MouseButtonState.Pressed;
            Mouse.OverrideCursor = Cursors.None;

            YLine.Visibility = Visibility.Visible;
            XLine.Visibility = Visibility.Visible;

            var mousePos = e.GetPosition(RgbMatrixCanvas);
            DrawLineCross(mousePos);
        }

        private void RgbMatrixCanvas_OnMouseLeave(object sender, MouseEventArgs e)
        {
            _isMouseDown = false;
            Mouse.OverrideCursor = Cursors.Arrow;

            YLine.Visibility = Visibility.Collapsed;
            XLine.Visibility = Visibility.Collapsed;
        }

        private void SetColorFromPosition(Point mousePos)
        {
            mousePos.X *= _source.PixelWidth / RgbMatrixImage.ActualWidth * 0.999;
            mousePos.Y *= _source.PixelHeight / RgbMatrixImage.ActualHeight * 0.999;

            var pixel = new byte[4];
            try
            {
                var cb = new CroppedBitmap(_source,
                    new Int32Rect((int)mousePos.X, (int)mousePos.Y, 1, 1));
                cb.CopyPixels(pixel, 4, 0);
            }
            catch (Exception)
            {
                // ignored
            }
            _selectedColor = Color.FromArgb(255, pixel[2], pixel[1], pixel[0]);
        }

        private void DrawLineCross(Point mousePos)
        {
            YLine.X1 = mousePos.X;
            YLine.X2 = mousePos.X;
            YLine.Y1 = 0;
            YLine.Y2 = RgbMatrixCanvas.ActualHeight;

            XLine.X1 = 0;
            XLine.X2 = RgbMatrixCanvas.ActualWidth;
            XLine.Y1 = mousePos.Y;
            XLine.Y2 = mousePos.Y;
        }
    }
}
