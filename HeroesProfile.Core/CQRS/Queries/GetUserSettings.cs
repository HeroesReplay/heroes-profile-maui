using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;

using MediatR;

namespace HeroesProfile.Core.CQRS.Queries;


public static class GetUserSettings
{
    public record Query : IRequest<Response>;

    public record Response(UserSettings UserSettings);

    public class Handler : IRequestHandler<Query, Response>
    {
        private readonly UserSettingsRepository repository;

        public Handler(UserSettingsRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            return new(await repository.LoadAsync(cancellationToken));
        }
    }
}