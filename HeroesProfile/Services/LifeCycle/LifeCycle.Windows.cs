using System.Runtime.InteropServices;
using HeroesProfile.UI.Core.Models;
using HeroesProfile.UI.Core.Repositories;
using Microsoft.Maui.LifecycleEvents;
using WinRT.Interop;

namespace HeroesProfile.UI.Services.LifeCycle;

public static class Lifecycle
{
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    private const int SW_HIDE = 0;
    private const int SW_SHOW = 5;
    private const int SW_MINIMIZE = 6;

    public static void MinimizeWindow(Microsoft.UI.Xaml.Window window)
    {
        ShowWindow(WindowNative.GetWindowHandle(window), SW_MINIMIZE);
    }

    public static void HideWindow(Microsoft.UI.Xaml.Window window)
    {
        ShowWindow(WindowNative.GetWindowHandle(window), SW_HIDE);
    }

    public static void ShowWindow(Microsoft.UI.Xaml.Window window)
    {
        ShowWindow(WindowNative.GetWindowHandle(window), SW_SHOW);
    }
    
    public static void Configure(IWindowsLifecycleBuilder configure)
    {
        configure.OnWindowCreated(x =>
        {
            
        });

        configure.OnLaunched((app, args) =>
        {
            Initializer.Start();
        });

        configure.OnVisibilityChanged((window, args) =>
        {   
            var userSettingsRepository = IPlatformApplication.Current!.Services.GetRequiredService<UserSettingsRepository>();

            var userSettings = userSettingsRepository.Load();

            if (args.Visible)
            {
                if(userSettings.EnableMinimizeToTray is true)
                {
                    ShowWindow(window);
                }
            }
            else
            {
                if (userSettings.EnableMinimizeToTray && args.Visible is false)
                {
                    HideWindow(window);
                }
                else if (userSettings.EnableMinimizeToTray is false && args.Visible is false)
                {
                    MinimizeWindow(window);
                }
            }
            
            
            
        });

        configure.OnClosed((window, args) =>
        {   
            Initializer.Stop();
        });

        configure.OnLaunching((app, args) =>
        {

        });

        configure.OnActivated((window, args) =>
        {

        });
    }

    public static void AddPlatformEvents(ILifecycleBuilder lifecycle)
    {
        lifecycle.AddWindows(Configure);
    }
}