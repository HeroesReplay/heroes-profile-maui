using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;

using MediatR;

namespace HeroesProfile.Core.CQRS.Commands
{
    public static class UpdateReplays
    {
        public record Response(IEnumerable<StoredReplay> Updated);

        public record Command(params StoredReplay[] Replays) : IRequest<Response>;

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly ReplaysRepository repository;

            public Handler(ReplaysRepository repository)
            {
                this.repository = repository;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                return new(await repository.UpdateAsync(request.Replays, cancellationToken));
            }
        }
    }
}