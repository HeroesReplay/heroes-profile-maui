using System.Collections.Generic;
using HeroesProfile.Core.Models;

using MediatR;

namespace HeroesProfile.Core.CQRS.Notifications;

public static class StoredReplaysUpdated
{
    public record Notification(IEnumerable<StoredReplay> storedReplays) : INotification;
}