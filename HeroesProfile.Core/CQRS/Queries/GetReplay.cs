using System.Threading;
using System.Threading.Tasks;

using MauiApp2.Core.Models;
using MauiApp2.Core.Repositories;

using MediatR;

namespace MauiApp2.Core.CQRS.Queries
{
    public static class GetReplay
    {
        public record Query(string Path) : IRequest<Response>;

        public record Response(StoredReplay StoredReplay);

        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly ReplaysRepository repository;

            public Handler(ReplaysRepository repository)
            {
                this.repository = repository;
            }

            public async Task<Response> Handle(Query query, CancellationToken cancellationToken)
            {
                var replay = await repository.FindAsync(query.Path, cancellationToken);
                return new(replay);
            }
        }
    }
}