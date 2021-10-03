using System;

using MauiApp2.Services;

namespace MauiApp2.Platforms.Windows
{
    public class WindowsTrayService : ITrayService
    {
        private WindowsTrayIcon tray;

        public Action ClickHandler { get; set; }

        public void Initialize()
        {
            tray = new WindowsTrayIcon("Platforms/Windows/Images/logo.ico")
            {
                LeftClick = () =>
                {
                    Microsoft.Maui.MauiWinUIApplication.Current.MainWindow.Activate();
                    ClickHandler?.Invoke();
                }
            };
        }
    }
}
