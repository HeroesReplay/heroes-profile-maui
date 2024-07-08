using HeroesProfile.UI.Core.CQRS.Notifications;
using HeroesProfile.UI.Core.Models;
using HeroesProfile.UI.Core.Repositories;
using MediatR;

namespace HeroesProfile.UI.Core.CQRS.Commands.Replays;

public static class UpdateReplays
{
    public record Response(IEnumerable<StoredReplay> Updated);

    public record Command(List<StoredReplay> Replays) : IRequest<Response>;

    public class Handler(ReplaysRepository repository, IMediator mediator) : IRequestHandler<Command, Response>
    {
        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var replays = await repository.UpdateAsync(request.Replays, cancellationToken);
            await mediator.Publish(new StoredReplaysUpdated.Notification(replays), cancellationToken);
            return new(replays);
        }
    }
}