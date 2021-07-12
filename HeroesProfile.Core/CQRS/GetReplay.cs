using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Services.Repositories;
using MediatR;

namespace HeroesProfile.Core.CQRS
{
    public static class GetReplay
    {
        public record Request(string Path) : IRequest<Response>;

        public record Response(StoredReplay StoredReplay);

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly ReplaysRepository repository;

            public Handler(ReplaysRepository repository)
            {
                this.repository = repository;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var replay = await repository.FindAsync(request.Path, cancellationToken);
                return new(replay);
            }
        }
    }
}