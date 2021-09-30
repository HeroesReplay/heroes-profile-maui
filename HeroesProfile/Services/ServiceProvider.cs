using Microsoft.Maui;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace MauiApp2.Services
{
    public static class ServiceProvider
    {
        public static TService GetService<TService>() => Current.GetService<TService>();

        public static IServiceProvider Current =>
#if WINDOWS
            MauiWinUIApplication.Current.Services;
#elif MACCATALYST
            MauiUIApplicationDelegate.Current.Services;
#else
			null;
#endif
    }
}