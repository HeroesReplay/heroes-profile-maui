using HeroesProfile.UI.Core.Models;
using HeroesProfile.UI.Core.Repositories;
using MediatR;

namespace HeroesProfile.UI.Core.CQRS.Queries;

public static class GetReplay
{
    public record Query(string Path) : IRequest<Response>;

    public record Response(StoredReplay StoredReplay);

    public class Handler(ReplaysRepository repository) : IRequestHandler<Query, Response>
    {
        public async Task<Response> Handle(Query query, CancellationToken cancellationToken)
        {
            var replay = await repository.FindAsync(query.Path, cancellationToken);
            return new(replay);
        }
    }
}