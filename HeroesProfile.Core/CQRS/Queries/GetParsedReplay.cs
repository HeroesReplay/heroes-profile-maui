using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Heroes.ReplayParser;

using MauiApp2.Core.Models;
using MauiApp2.Core.Parsers;

using MediatR;

namespace MauiApp2.Core.CQRS.Queries
{
    public static class GetParsedReplay
    {
        public record Response(ReplayParseData Data);

        public record Query(FileInfo File, ParseOptions options) : IRequest<Response>;

        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly AggregateReplayParser replayParser;

            public Handler(AggregateReplayParser replayParser)
            {
                this.replayParser = replayParser;
            }

            public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                var options = request.options ?? new ParseOptions
                {
                    /* 
                     * We do care?
                     */
                    ShouldParseDetailedBattleLobby = true,
                    ShouldParseEvents = true,

                    /* 
                     * We dont care?
                     */
                    ShouldParseUnits = false,
                    ShouldParseMessageEvents = false,
                    ShouldParseMouseEvents = false,
                    ShouldParseStatistics = false,
                    AllowPTR = false,
                    IgnoreErrors = false,
                };

                return new Response(await replayParser.ParseAsync(request.File, options, cancellationToken));
            }
        }
    }
}