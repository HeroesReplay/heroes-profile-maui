using HeroesProfile.Core.Models;
using MediatR;

namespace HeroesProfile.Core.CQRS.Notifications;

public static class TwitchPredictionUpdated
{
    public record Notification(SessionData SessionData) : INotification;
}