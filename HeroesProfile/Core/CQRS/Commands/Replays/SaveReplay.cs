using HeroesProfile.UI.Core.CQRS.Notifications;
using HeroesProfile.UI.Core.Models;
using HeroesProfile.UI.Core.Repositories;
using MediatR;

namespace HeroesProfile.UI.Core.CQRS.Commands.Replays;

public static class SaveReplay
{
    public record Command(ReplayParseData ParseData) : IRequest<Response>;

    public record Response(StoredReplay StoredReplay);

    public class Handler(ReplaysRepository replaysRepository, IMediator mediator) : IRequestHandler<Command, Response>
    {
        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var storedReplay = StoredReplay.From(request.ParseData);
            await replaysRepository.UpdateAsync([storedReplay], cancellationToken);
            await mediator.Publish(new StoredReplaysUpdated.Notification([storedReplay]), cancellationToken);
            return new Response(storedReplay);
        }
    }
}