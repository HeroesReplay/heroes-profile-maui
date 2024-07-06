using HeroesProfile.UI.Services.Notification;
using Microsoft.Toolkit.Uwp.Notifications;
namespace HeroesProfile.UI.Services.Notification;

public class PlatformNotificationService : IPlatformNotificationService
{
    public void ShowNotification(string title, string subtitle, string body)
    {
        new ToastContentBuilder()
            .AddToastActivationInfo(null, ToastActivationType.Foreground)
            .AddAppLogoOverride(new Uri("ms-appx:///Assets/dotnet_bot.png"))
            .AddText(title, hintStyle: AdaptiveTextStyle.Header)
            .AddText(subtitle, hintStyle: AdaptiveTextStyle.Subtitle)
            .AddText(body, hintStyle: AdaptiveTextStyle.Body)
            .Show();
    }
}
