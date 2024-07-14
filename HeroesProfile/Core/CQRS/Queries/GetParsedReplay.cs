using HeroesProfile.UI.Core.Models;
using HeroesProfile.UI.Core.Parsers;
using MediatR;

namespace HeroesProfile.UI.Core.CQRS.Queries;

public static class GetParsedReplay
{
    public record Response(ReplayParseData Data);

    public record Query(FileInfo File) : IRequest<Response>;

    public class Handler(AggregateReplayParser replayParser) : IRequestHandler<Query, Response>
    {
        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            ReplayParseData parseData = await replayParser.ParseAsync(request.File, cancellationToken);
            return new Response(parseData);
        }
    }
}