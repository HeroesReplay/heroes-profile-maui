using System;
using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.Clients;
using HeroesProfile.Core.CQRS.Notifications;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;

using MediatR;

namespace HeroesProfile.Core.CQRS.Commands.Twitch;

public static class CreateTalents
{
    public record Command : IRequest<Response>;

    public record Response(SessionData Session);

    public class Handler : IRequestHandler<Command, Response>
    {
        private readonly SessionRepository sessionRepository;
        private readonly TalentsClient talentsClient;
        private readonly UserSettingsRepository userSettingsRepository;
        private readonly IMediator mediator;

        public Handler(SessionRepository sessionRepository, TalentsClient talentsClient, UserSettingsRepository userSettingsRepository, IMediator mediator)
        {
            this.sessionRepository = sessionRepository;
            this.talentsClient = talentsClient;
            this.userSettingsRepository = userSettingsRepository;
            this.mediator = mediator;
        }

        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            Models.UserSettings userSettings = await userSettingsRepository.LoadAsync(cancellationToken);

            var sessionId = await talentsClient.CreateSession(userSettings.Identity, cancellationToken);

            if (!string.IsNullOrWhiteSpace(sessionId))
            {
                sessionRepository.SessionData.TalentsExtension.SessionId = sessionId;
                sessionRepository.SessionData.TalentsExtension.LastUpdate = DateTime.Now;

                await talentsClient.SavePlayerData(userSettings.Identity, sessionRepository.SessionData, cancellationToken);
                await talentsClient.NotifyTwitchTalentChange(userSettings.Identity, cancellationToken);
                await mediator.Publish(new TwitchTalentsUpdated.Notification(sessionRepository.SessionData), cancellationToken);
            }


            return new Response(sessionRepository.SessionData);
        }
    }
}