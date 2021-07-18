using System;


namespace HeroesProfile.UI.Maui.Platforms.Windows
{
    public class TrayService : ITrayService
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
