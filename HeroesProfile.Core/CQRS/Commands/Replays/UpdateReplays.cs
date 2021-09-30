using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MauiApp2.Core.CQRS.Notifications;
using MauiApp2.Core.Models;
using MauiApp2.Core.Repositories;

using MediatR;

namespace MauiApp2.Core.CQRS.Commands.Replays
{
    public static class UpdateReplays
    {
        public record Response(IEnumerable<StoredReplay> Updated);

        public record Command(params StoredReplay[] Replays) : IRequest<Response>;

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly ReplaysRepository repository;
            private readonly IMediator mediator;

            public Handler(ReplaysRepository repository, IMediator mediator)
            {
                this.repository = repository;
                this.mediator = mediator;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                var replays = await repository.UpdateAsync(request.Replays, cancellationToken);

                await mediator.Publish(new StoredReplaysUpdated.Notification(replays), cancellationToken);

                return new(replays);
            }
        }
    }
}