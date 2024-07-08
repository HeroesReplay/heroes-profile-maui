using Microsoft.Maui.LifecycleEvents;

namespace HeroesProfile.UI.Services.LifeCycle;

public static class Lifecycle
{
    public static void Configure(IWindowsLifecycleBuilder configure)
    {
        configure.OnWindowCreated(x =>
        {
            x.Title = "Heroes Profile - Alpha";
        });

        configure.OnLaunched((app, args) =>
        {
            Initializer.Start();
        });

        configure.OnVisibilityChanged((window, args) =>
        {
            if (!args.Visible)
            {
                //window.MinimizeToTray();
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