using Microsoft.Toolkit.Uwp.Notifications;

using System;

namespace HeroesProfile.Maui.Platforms.Windows
{
    public class WindowsNotificationService : IOSNotificationService
    {
        public void ShowNotification(string title, string body)
        {
            new ToastContentBuilder()
                .AddToastActivationInfo(null, ToastActivationType.Foreground)
                .AddAppLogoOverride(new Uri("ms-appx:///Assets/dotnet_bot.png"))
                .AddText(title, hintStyle: AdaptiveTextStyle.Header)
                .AddText(body, hintStyle: AdaptiveTextStyle.Body)
                .Show();
        }
    }
}
