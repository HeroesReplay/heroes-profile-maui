using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using HeroesProfile.Core.Models;
using HeroesProfile.Core.Services.Repositories;

using MediatR;

namespace HeroesProfile.Core.CQRS
{
    public static class GetReplays
    {
        public record Filter(ParseResult ParseResult, ProcessStatus ProcessStatus);

        public record Request(List<Filter> filters) : IRequest<Response>;

        public record Response(IEnumerable<StoredReplay> Replays);

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly ReplaysRepository repository;

            public Handler(ReplaysRepository repository)
            {
                this.repository = repository;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                List<StoredReplay> replays = await repository.LoadAsync(cancellationToken);

                if (request.filters == null || request.filters.Count == 0)
                {
                    return new(replays);
                }

                return new(replays.Where(replay => request.filters.Any(filter => replay.ParseResult == filter.ParseResult && replay.ProcessStatus == filter.ProcessStatus)).ToList());
            }
        }
    }
}