namespace MauiApp2.Services
{
    public interface INotificationService
    {
        void ShowNotification(string title, string subtitle, string body);
    }
}