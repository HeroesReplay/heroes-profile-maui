using HeroesProfile.UI.Services;

namespace HeroesProfile.UI.Platforms.Windows;

public class WindowsTrayService : ITrayService
{
    private WindowsTrayIcon tray;

    public WindowsTrayService()
    {
        
    }

    public Action ClickHandler { get; set; }

    public void Initialize()
    {
        tray = new WindowsTrayIcon("Platforms/Windows/Images/logo.ico")
        {
            LeftClick = () =>
            {
                // Microsoft.Maui.MauiWinUIApplication.Current.Application.Windows[0].Activate();
                ClickHandler?.Invoke();
            }
        };
    }
}
