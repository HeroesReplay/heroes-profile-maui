using System.Collections.Generic;

using MauiApp2.Core.Models;

using MediatR;

namespace MauiApp2.Core.CQRS.Notifications
{
    public static class StoredReplaysUpdated
    {
        public record Notification(IEnumerable<StoredReplay> storedReplays) : INotification;
    }
}