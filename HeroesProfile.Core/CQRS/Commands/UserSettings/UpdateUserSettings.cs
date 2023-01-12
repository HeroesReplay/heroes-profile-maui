using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.Repositories;
using MediatR;

namespace HeroesProfile.Core.CQRS.Commands.UserSettings;

public static class UpdateUserSettings
{
    public record Command(Models.UserSettings Settings) : IRequest<Response>;

    public record Response(Models.UserSettings Settings);

    public class Handler : IRequestHandler<Command, Response>
    {
        private readonly UserSettingsRepository repository;

        public Handler(UserSettingsRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            await repository.SaveAsync(request.Settings, cancellationToken);
            return new Response(request.Settings);
        }
    }
}