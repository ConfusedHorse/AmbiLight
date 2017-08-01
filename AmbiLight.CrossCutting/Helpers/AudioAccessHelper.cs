using System;
using System.Diagnostics;
using NAudio.CoreAudioApi;
using MMDeviceEnumerator = NAudio.CoreAudioApi.MMDeviceEnumerator;

namespace AmbiLight.CrossCutting.Helpers
{
    public static class AudioAccessHelper
    {
        #region Fields

        private static readonly MMDeviceEnumerator MmDeviceEnumerator = new MMDeviceEnumerator();

        #endregion

        #region private methods

        private static MMDevice GetDefaultDevice(DataFlow kind, Role role = Role.Multimedia)
        {
            try
            {
                var dev = MmDeviceEnumerator.GetDefaultAudioEndpoint(kind, role);
                return dev;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Could not aquire default device due to an excepion: " + ex.Message);
                return null;
            }
        }

        #endregion private methods

        #region public methods

        public static MMDeviceCollection GetActiveOutoutDevices()
        {
            return GetActiveDevices(DataFlow.Render);
        }

        public static MMDeviceCollection GetActiveInputDevices()
        {
            return GetActiveDevices(DataFlow.Capture);
        }

        public static MMDevice GetDefaultOutputDevice(Role role)
        {
            return GetDefaultDevice(DataFlow.Render, role);
        }

        public static MMDevice GetDefaultInputDevice(Role role)
        {
            return GetDefaultDevice(DataFlow.Capture, role);
        }

        public static bool IsDefault(this MMDevice device, DataFlow flow = DataFlow.Render, Role role = Role.Multimedia)
        {
            return GetDefaultDevice(flow, role).FriendlyName == device.FriendlyName; //cannot access id
        }

        public static MMDevice ToDevice(this string pwstrDeviceId)
        {
            try
            {
                var device = MmDeviceEnumerator.GetDevice(pwstrDeviceId);
                return device;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Could not enumerate devices due to an excepion: " + ex.Message);
                return null;
            }
        }

        private static MMDeviceCollection GetActiveDevices(DataFlow kind)
        {
            try
            {
                var devCol = MmDeviceEnumerator.EnumerateAudioEndPoints(kind, DeviceState.Active);
                return devCol;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Could not enumerate devices due to an excepion: " + ex.Message);
                return null;
            }
        }

        #endregion public methods
    }
}