using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;
using MediatR;

namespace HeroesProfile.Core.CQRS.Commands
{
    public static class ClearSession
    {
        public record Command(string FileToCopy) : IRequest<Response>;

        public record Response();

        public class Handler : IRequestHandler<Command, Response>
        {
            private AppSettings appSettings;
            private readonly SessionRepository sessionRepository;

            public Handler(AppSettings appSettings, SessionRepository sessionRepository)
            {
                this.appSettings = appSettings;
                this.sessionRepository = sessionRepository;
            }

            public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
            {
                await sessionRepository.ClearAsync(cancellationToken);

                return new Response();
            }
        }
    }
}