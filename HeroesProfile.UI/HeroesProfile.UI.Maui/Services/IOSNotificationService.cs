using System;
using System.Threading.Tasks;

using Blazorise;

using Microsoft.AspNetCore.Components;

namespace HeroesProfile.UI.Maui
{
    public interface IOSNotificationService
    {
        void ShowNotification(string title, string body);
    }
}