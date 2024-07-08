using HeroesProfile.UI.Core.CQRS.Notifications;
using HeroesProfile.UI.Core.Models;
using HeroesProfile.UI.Core.Repositories;
using MediatR;

namespace HeroesProfile.UI.Core.CQRS.Commands.Replays;

public static class SaveReplays
{
    public record Command(params ReplayParseData[] ParseDatas) : IRequest<Response>;

    public record Response(List<StoredReplay> StoredReplays);

    public class Handler(ReplaysRepository replaysRepository, IMediator mediator) : IRequestHandler<Command, Response>
    {
        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var storedReplays = request.ParseDatas.Select(data => new StoredReplay()
            {
                Updated = DateTime.UtcNow,
                ProcessStatus = data.ProcessStatus,
                ParseStatus = data.ParseStatus,
                Created = data.Replay?.Timestamp ?? data.File.CreationTime,
                Path = data.File.FullName,
                Fingerprint = data.Fingerprint,
            })
            .ToList();

            await replaysRepository.InsertAsync(storedReplays, cancellationToken);
            await mediator.Publish(new StoredReplaysUpdated.Notification(storedReplays), cancellationToken);
            return new Response(storedReplays);
        }
    }
}