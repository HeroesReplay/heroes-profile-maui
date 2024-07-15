using HeroesProfile.UI.Core.Repositories;
using MediatR;

namespace HeroesProfile.UI.Core.CQRS.Commands.UserSettings;

public static class UpdateUserSettings
{
    public record Command(Models.UserSettings Settings) : IRequest<Response>;

    public record Response(Models.UserSettings Settings);

    public class Handler(UserSettingsRepository repository) : IRequestHandler<Command, Response>
    {
        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            repository.Save(request.Settings);
            return new Response(request.Settings);
        }
    }
}