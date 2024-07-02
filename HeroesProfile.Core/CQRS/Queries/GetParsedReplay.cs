using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Heroes.ReplayParser;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Parsers;

using MediatR;

namespace HeroesProfile.Core.CQRS.Queries;

public static class GetParsedReplay
{
    public record Response(ReplayParseData Data);

    public record Query(FileInfo File, Heroes.StormReplayParser.ParseOptions options) : IRequest<Response>;

    public class Handler : IRequestHandler<Query, Response>
    {
        private readonly AggregateReplayParser replayParser;

        public Handler(AggregateReplayParser replayParser)
        {
            this.replayParser = replayParser;
        }

        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            var options = request.options ?? new Heroes.StormReplayParser.ParseOptions
            {
               AllowPTR = true,
               ShouldParseGameEvents = false,
               ShouldParseMessageEvents = false,
               ShouldParseTrackerEvents = false
            };

            return new Response(await replayParser.ParseAsync(request.File, options, cancellationToken));
        }
    }
}