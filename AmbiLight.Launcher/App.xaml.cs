using System;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using AmbiLight.CrossCutting.Module;
using AmbiLight.View.Controls;
using AmbiLight.View.Module;
using AmbiLight.View.Resources;
using BlurryControls.DialogFactory;
using BlurryControls.Internals;
using GalaSoft.MvvmLight.CommandWpf;
using Hardcodet.Wpf.TaskbarNotification;
using Ninject;

namespace AmbiLight.Launcher
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        #region Behaviour

        public TaskbarIcon TaskbarIcon { get; set; }
        private static readonly Mutex SingleInstance = new Mutex(true, "AmbiLightSingleInstanceMutex");

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!SingleInstance.WaitOne(TimeSpan.Zero, true))
            {
                BlurryMessageBox.Show(
                    View.Properties.Resources.AlreadyRunningMessage,
                    View.Properties.Resources.AlreadyRunningCaption,
                    BlurryDialogButton.Ok,
                    BlurryDialogIcon.Error
                );
                Current.Shutdown();
            }

            //Initialize tray icon
            TaskbarIcon = new TaskbarIcon();

            //Force rendering (avoid initial delay)
            ViewModelLocator.Instance.AmbiLightTray.ForceRendering();

            //Initialize appearance
            TaskbarIcon.Icon = Icons.Rgb16;

            //Initialize behaviour
            TaskbarIcon.ContextMenu = ContextMenus.TrayContextMenu();
            TaskbarIcon.DoubleClickCommand = null; //TODO implement options ? start/stop ?
            TaskbarIcon.LeftClickCommand = new RelayCommand(TrayOnLeftClick);
            
            base.OnStartup(e);
        }

        private static void TrayOnLeftClick()
        {
            var ambiLightTray = AmbiLightKernel.Instance.Get<AmbiLightTray>();
            ambiLightTray.ShowHide();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Thread.Sleep(100); //await running colors
            ViewModelLocator.Instance.AmbiLightViewModel.PostEmpty();
            ViewModelLocator.Instance.AmbiLightViewModel.SaveSettings();

            try { SingleInstance.ReleaseMutex(); } catch (Exception) { /* bla */ }
            
            StartOnWindowsStartUp();

            base.OnExit(e);
        }

        private static void StartOnWindowsStartUp()
        {
            try
            {
                var key =
                Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                    "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                var curAssembly = Assembly.GetExecutingAssembly();


                if (ViewModel.Properties.Settings.Default.StartOnWindowsStartup)
                {
                    key?.SetValue(curAssembly.GetName().Name, curAssembly.Location);
                }
                else
                {
                    key?.DeleteValue(curAssembly.GetName().Name, false);
                }
            }
            catch (Exception) { /* bla */ }
        }

        #endregion Behaviour

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            //TODO ask if mailing the error to me is ok
            BlurryMessageBox.Show(
                $"{View.Properties.Resources.ErrorMessage}\r\n{e.Exception.Message}",
                View.Properties.Resources.ErrorCaption,
                BlurryDialogButton.Ok,
                BlurryDialogIcon.Error
            );
            Current.Shutdown();
        }
    }
}
