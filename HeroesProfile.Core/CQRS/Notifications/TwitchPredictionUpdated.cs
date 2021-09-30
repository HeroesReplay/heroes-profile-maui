
using MauiApp2.Core.Models;

using MediatR;

namespace MauiApp2.Core.CQRS.Notifications
{
    public static class TwitchPredictionUpdated
    {
        public record Notification(SessionData SessionData) : INotification;
    }
}