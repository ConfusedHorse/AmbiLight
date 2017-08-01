using System.Windows;
using System.Windows.Controls;
using AmbiLight.View.Resources;
using GalaSoft.MvvmLight.CommandWpf;

namespace AmbiLight.View.Controls
{
    public static class ContextMenus
    {
        public static ContextMenu TrayContextMenu()
        {
            var contextMenu = new ContextMenu
            {
                Width = 200d
            };

            //close application
            var closeMenuItem = new MenuItem
            {
                Icon = Icons.Close16,
                Header = Properties.Resources.ShutDownHeader,
                Command = new RelayCommand(() => Application.Current.Shutdown())
            };

            contextMenu.Items.Add(closeMenuItem);

            return contextMenu;
        } 
    }
}