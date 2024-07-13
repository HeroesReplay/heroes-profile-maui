using Microsoft.Maui.LifecycleEvents;

namespace HeroesProfile.UI.Services.LifeCycle;

public static class Lifecycle
{
    public static void Configure(IiOSLifecycleBuilder configure)
    {
        configure.DidEnterBackground(OnBackground);
        configure.WillEnterForeground(OnForeground);
        configure.OnActivated(OnActivated);
        configure.WillTerminate(OnTerminate);

    }

    private static void OnTerminate(UIKit.UIApplication application)
    {
         Initializer.Stop();
    }

    private static void OnActivated(UIKit.UIApplication application)
    {
        // User select folder for permissions
        

        
        Initializer.Start();


    }

    private static void OnForeground(UIKit.UIApplication application)
    {
        
    }

    private static void OnBackground(UIKit.UIApplication application)
    {
        
    }

    public static void AddPlatformEvents(ILifecycleBuilder lifecycle)
    {
        lifecycle.AddiOS(Configure);
    }
}
