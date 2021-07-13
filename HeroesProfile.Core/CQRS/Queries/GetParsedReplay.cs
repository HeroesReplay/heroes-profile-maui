using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Parsers;
using MediatR;

namespace HeroesProfile.Core.CQRS.Queries
{
    public static class GetParsedReplay
    {
        public record Response(ReplayParseData Data);

        public record Query(FileInfo File) : IRequest<Response>;

        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly AggregateReplayParser replayParser;

            public Handler(AggregateReplayParser replayParser)
            {
                this.replayParser = replayParser;
            }

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                ReplayParseData result = await replayParser.ParseAsync(request.File, cancellationToken);
                return new Response(result);
            }
        }
    }
}