using Microsoft.Maui;

namespace HeroesProfile.UI.Services;

public static class ServiceProvider
{
#if WINDOWS
    public static IPlatformApplication Current => HeroesProfile.WinUI.App.Current;
#endif

#if MACCATALYST
    public static IPlatformApplication Current => HeroesProfile.UI.AppDelegate.Current;
#endif
}