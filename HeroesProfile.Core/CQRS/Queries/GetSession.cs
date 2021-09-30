using System.Threading;
using System.Threading.Tasks;

using MauiApp2.Core.Models;
using MauiApp2.Core.Repositories;

using MediatR;

namespace MauiApp2.Core.CQRS.Queries
{


    public static class GetSession
    {
        public record Response(SessionData Session);

        public record Query : IRequest<Response>;

        public class Handler : IRequestHandler<Query, Response>
        {
            private readonly SessionRepository sessionManager;

            public Handler(SessionRepository sessionManager)
            {
                this.sessionManager = sessionManager;
            }

            public Task<Response> Handle(Query request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new Response(sessionManager.SessionData));
            }
        }
    }
}