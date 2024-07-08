using Heroes.StormReplayParser;
using HeroesProfile.UI.Core.Models;
using HeroesProfile.UI.Core.Repositories;
using MediatR;

namespace HeroesProfile.UI.Core.CQRS.Queries;

public static class GetReplays
{
    public record Filter(ProcessStatus ProcessStatus, StormReplayParseStatus ParseStatus);

    public record Query(List<Filter>? Filters = null) : IRequest<Response>;

    public record Response(List<StoredReplay> Replays);

    public class Handler(ReplaysRepository repository) : IRequestHandler<Query, Response>
    {
        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            List<StoredReplay> replays = await repository.LoadAsync(cancellationToken);

            if (request.Filters == null || request.Filters.Count == 0) return new Response(replays);

            var filtered = replays.Where(replay =>
            {
                return request.Filters.Any(filter =>
                    replay.ProcessStatus == filter.ProcessStatus &&
                    replay.ParseStatus == filter.ParseStatus);
            });

            return new Response(filtered.ToList());
        }
    }
}