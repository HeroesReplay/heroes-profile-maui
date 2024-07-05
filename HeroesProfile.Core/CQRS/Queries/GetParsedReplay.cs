using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Parsers;
using MediatR;

namespace HeroesProfile.Core.CQRS.Queries;

public static class GetParsedReplay
{
    public record Response(ReplayParseData Data);

    public record Query(FileInfo File, Heroes.StormReplayParser.ParseOptions Options) : IRequest<Response>;

    public class Handler(AggregateReplayParser replayParser) : IRequestHandler<Query, Response>
    {
        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            var parseData = await replayParser.ParseAsync(request.File, request.Options, cancellationToken);
            return new Response(parseData);
        }
    }
}