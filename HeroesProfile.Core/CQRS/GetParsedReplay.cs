using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Services.Parsers;
using MediatR;

namespace HeroesProfile.Core.CQRS
{
    public static class GetParsedReplay
    {
        public record Response(ReplayParseData Data);

        public record Request(FileInfo File) : IRequest<Response>;

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly AggregateReplayParser replayParser;

            public Handler(AggregateReplayParser replayParser)
            {
                this.replayParser = replayParser;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                ReplayParseData result = await replayParser.ParseAsync(request.File, cancellationToken);
                return new Response(result);
            }
        }
    }
}