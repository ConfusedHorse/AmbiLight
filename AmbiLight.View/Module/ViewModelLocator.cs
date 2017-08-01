using System.ComponentModel;
using AmbiLight.CrossCutting.Helpers;
using AmbiLight.CrossCutting.Module;
using AmbiLight.View.Controls;
using AmbiLight.ViewModel.Models;
using Ninject;

namespace AmbiLight.View.Module
{
    public class ViewModelLocator
    {
        #region Singleton

        private static ViewModelLocator _instance;

        public static ViewModelLocator Instance
        {
            get
            {
                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return null;
                return _instance ?? (_instance = new ViewModelLocator());
            }
        }

        #endregion Singleton

        // ViewModel
        public AmbiLightViewModel AmbiLightViewModel => AmbiLightKernel.Instance.Get<AmbiLightViewModel>();
        public ScreenHelper ScreenHelper => AmbiLightKernel.Instance.Get<ScreenHelper>();

        // View
        public AmbiLightOptions AmbiLightOptions => AmbiLightKernel.Instance.Get<AmbiLightOptions>();
        public AmbiLightTray AmbiLightTray => AmbiLightKernel.Instance.Get<AmbiLightTray>();

        public static void Cleanup()
        {

        }
    }
}