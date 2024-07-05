using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;

using MediatR;

namespace HeroesProfile.Core.CQRS.Queries;



public static class GetSession
{
    public record Response(SessionData Session);

    public record Query : IRequest<Response>;

    public class Handler(SessionRepository sessionManager) : IRequestHandler<Query, Response>
    {
        public Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Response(sessionManager.SessionData));
        }
    }
}