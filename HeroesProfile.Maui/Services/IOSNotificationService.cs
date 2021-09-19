using System;
using System.Threading.Tasks;

using Blazorise;

using Microsoft.AspNetCore.Components;

namespace HeroesProfile.Maui
{
    public interface IOSNotificationService
    {
        void ShowNotification(string title, string body);
    }
}