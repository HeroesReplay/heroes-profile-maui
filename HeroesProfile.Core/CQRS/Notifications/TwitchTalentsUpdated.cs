using HeroesProfile.Core.Models;
using MediatR;

namespace HeroesProfile.Core.CQRS.Notifications;

public static class TwitchTalentsUpdated
{
    public record Notification(SessionData SessionData) : INotification;
}