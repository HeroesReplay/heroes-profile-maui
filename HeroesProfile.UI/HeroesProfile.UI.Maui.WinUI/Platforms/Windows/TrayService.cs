using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeroesProfile.UI.Maui.Platforms.Windows
{
    public class TrayService : ITrayService
    {
        // WindowsTrayIcon tray;

        public Action ClickHandler { get; set; }

        public void Initialize()
        {
            //tray = new WindowsTrayIcon("Platforms/Windows/trayicon.ico");
            //tray.LeftClick = () =>
            //{
            //	Microsoft.Maui.MauiWinUIApplication.Current.MainWindow.BringToFront();
            //	ClickHandler?.Invoke();
            //};
        }
    }
}
