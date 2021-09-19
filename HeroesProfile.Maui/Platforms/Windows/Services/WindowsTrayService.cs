using System;

namespace HeroesProfile.Maui.Platforms.Windows
{
    public class WindowsTrayService : ITrayService
    {
        private WindowsTrayIcon tray;

        public Action ClickHandler { get; set; }

        public void Initialize()
        {
            tray = new WindowsTrayIcon("Platforms/Windows/icon.ico")
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
