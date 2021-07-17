using System;

namespace HeroesProfile.UI.Maui
{
    public interface INotificationService
    {
        void ShowNotification(string title, string body);
    }
}