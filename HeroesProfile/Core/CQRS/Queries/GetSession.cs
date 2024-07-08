using HeroesProfile.UI.Core.Models;
using HeroesProfile.UI.Core.Repositories;
using MediatR;

namespace HeroesProfile.UI.Core.CQRS.Queries;



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