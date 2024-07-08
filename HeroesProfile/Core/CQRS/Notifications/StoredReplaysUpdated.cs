using HeroesProfile.UI.Core.Models;
using MediatR;

namespace HeroesProfile.UI.Core.CQRS.Notifications;

public static class StoredReplaysUpdated
{
    public record Notification(List<StoredReplay> StoredReplays) : INotification;
}