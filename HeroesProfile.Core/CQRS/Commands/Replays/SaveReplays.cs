using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MauiApp2.Core.CQRS.Notifications;
using MauiApp2.Core.Models;
using MauiApp2.Core.Repositories;

using MediatR;

namespace MauiApp2.Core.CQRS.Commands.Replays
{
    public static class SaveReplays
    {
        public record Command(params ReplayParseData[] ParseDatas) : IRequest<Response>;

        public record Response(List<StoredReplay> StoredReplays);

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly ReplaysRepository replaysRepository;
            private readonly IMediator mediator;

            public Handler(ReplaysRepository replaysRepository, IMediator mediator)
            {
                this.replaysRepository = replaysRepository;
                this.mediator = mediator;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                var storedReplays = request.ParseDatas.Select(data => new StoredReplay()
                {
                    Updated = DateTime.UtcNow,
                    Created = data.ParseResult == ParseResult.Success ? data.Replay.Timestamp : data.File.CreationTime,
                    Path = data.File.FullName,
                    ParseResult = data.ParseResult,
                    Fingerprint = data.Fingerprint,
                    ProcessStatus = ProcessStatus.Pending
                });

                var list = storedReplays.ToList();

                await replaysRepository.InsertAsync(list, cancellationToken);

                await mediator.Publish(new StoredReplaysUpdated.Notification(list), cancellationToken);

                return new Response(list);
            }
        }
    }


}