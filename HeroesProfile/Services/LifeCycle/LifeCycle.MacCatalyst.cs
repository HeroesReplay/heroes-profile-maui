using Microsoft.Maui.LifecycleEvents;

namespace HeroesProfile.UI.Services.LifeCycle;

public static class Lifecycle
{
    public static void Configure(IiOSLifecycleBuilder configure)
    {

    }

    public static void AddPlatformEvents(ILifecycleBuilder lifecycle)
    {
        lifecycle.AddiOS(Configure);
    }
}
