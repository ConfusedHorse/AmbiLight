using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Windows.Threading;

namespace AmbiLight.CrossCutting.Helpers
{
    public class CommunicationHelper
    {
        private readonly List<SerialPort> ports = new List<SerialPort>();
        private readonly DispatcherTimer _arduinoDispatcherTimer = new DispatcherTimer();
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
                ConnectToArduino(abbreviation);
        }

        private void ConnectToArduino(string abbreviation = "COM")
        {
            if (Connected) return;
            Debug.WriteLine("Trying to find Arduino...");
            _isFirstConnectionAttempt = false;
            Connected = false;

            var portNames = SerialPort.GetPortNames();
            if (portNames.Length <= 0) return;

            var possibleLedDrivers = portNames.Where(port => port.StartsWith(abbreviation));
            foreach (var portName in possibleLedDrivers)
            {
                var port = new SerialPort(portName, BaudRate);
                try
                {
                    Debug.WriteLine($"Prompting {portName}");

                    port.ReadTimeout = 20;
                    port.DataReceived += new SerialDataReceivedEventHandler(port_handleDiscovery);
                    port.Open();
                    port.WriteLine("init");
                    ports.Add(port);
                }
                catch (Exception e)
                {
                    if (e is TimeoutException)
                    {
                        Debug.WriteLine($"Timeout in {portName}");
                        port.Close();
                        continue;
                    }
                    _arduinoDispatcherTimer.Start();
                }
            }
        }

        private void port_handleDiscovery(object sender, SerialDataReceivedEventArgs e)
        {
            foreach (var port in ports.ToList())
            {
                var answer = port.ReadExisting();
                Debug.WriteLine($"Answer {answer}");
                if (answer == "ambilight")
                {
                    Debug.WriteLine($"Arduino found! ({port.PortName})");

                    _arduinoDispatcherTimer.Stop();
                    Connected = true;
                    _isFirstConnectionAttempt = true;
                }
                else
                {
                    port.Close();
                    ports.Remove(port);
                    continue;
                }
            }
        }

        public void SendDataToPort(string message)
        {
            if (!Connected)
            {
                _arduinoDispatcherTimer.Start();
            } 
            else
            {
                try
                {
                    ports.First()?.WriteLine(message);
                }
                catch (Exception)
                {
                    _arduinoDispatcherTimer.Start();
                }
            }
        }
    }
}