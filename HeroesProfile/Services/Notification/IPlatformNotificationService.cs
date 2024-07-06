namespace HeroesProfile.UI.Services.Notification;

public interface IPlatformNotificationService
{
    void ShowNotification(string title, string subtitle, string body);
}