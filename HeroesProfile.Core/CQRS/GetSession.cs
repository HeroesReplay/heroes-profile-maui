using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Services;
using MediatR;

namespace HeroesProfile.Core.CQRS
{
    public static class GetSession
    {
        public record GetSessionResponse(Session Session);

        public record Request : IRequest<GetSessionResponse>;

        public class Handler : IRequestHandler<Request, GetSessionResponse>
        {
            private readonly SessionManager sessionManager;

            public Handler(SessionManager sessionManager)
            {
                this.sessionManager = sessionManager;
            }

            public Task<GetSessionResponse> Handle(Request request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new GetSessionResponse(sessionManager.Session));
            }
        }
    }

  
}