using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Windows.Threading;

namespace AmbiLight.CrossCutting.Helpers
{
    public class CommunicationHelper
    {
        private SerialPort _port;
        private readonly DispatcherTimer _arduinoDispatcherTimer = new DispatcherTimer();
        private string _portName = string.Empty;
        private bool _isFirstConnectionAttempt;
        private const int BaudRate = 115200;

        public bool Connected { get; set; }

        public CommunicationHelper()
        {
            _arduinoDispatcherTimer.Interval = TimeSpan.FromMilliseconds(2000);
            _arduinoDispatcherTimer.Tick += (sender, args) => ConnectToArduino();
        }

        public void FindArduino(string abbreviation = "COM")
        {
            if (_arduinoDispatcherTimer.IsEnabled) return;
            Connected = false;

            if (_isFirstConnectionAttempt)
                _arduinoDispatcherTimer.Start();
            else
                ConnectToArduino();
        }

        private void ConnectToArduino(string abbreviation = "COM")
        {
            if (Connected) return;
            Debug.WriteLine("Trying to find Arduino...");
            _isFirstConnectionAttempt = false;
            Connected = false;

            var portNames = SerialPort.GetPortNames();
            if (portNames.Length <= 0) return;

            _portName = portNames.First(port => port.Contains(abbreviation));
            if (_portName == string.Empty) return;
            Debug.WriteLine($"Arduino found! ({_portName})");

            BeginSerial(BaudRate, _portName);
            try
            {
                _port.Open();
            }
            catch (Exception)
            {
                _arduinoDispatcherTimer.Start();
            }

            _arduinoDispatcherTimer.Stop();
            Connected = true;
            _isFirstConnectionAttempt = true;
        }

        public void SendDataToPort(string message)
        {
            try
            {
                _port.WriteLine(message);
            }
            catch (Exception)
            {
                _arduinoDispatcherTimer.Start();
            }
        }

        private void BeginSerial(int baud, string name)
        {
            _port = new SerialPort(name, baud);
        }
    }
}