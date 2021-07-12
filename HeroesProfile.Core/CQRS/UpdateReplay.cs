using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Heroes.ReplayParser;

using HeroesProfile.Core.Models;
using HeroesProfile.Core.Services.Repositories;

using MediatR;

namespace HeroesProfile.Core.CQRS
{
    public static class UpdateReplay
    {
        public record Response(StoredReplay Updated);

        public record Request(StoredReplay StoredReplay) : IRequest<Response>;

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly ReplaysRepository replaysRepository;

            public Handler(ReplaysRepository replaysRepository)
            {
                this.replaysRepository = replaysRepository;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                return new(await this.replaysRepository.UpdateAsync(request.StoredReplay, cancellationToken));
            }
        }
    }
}