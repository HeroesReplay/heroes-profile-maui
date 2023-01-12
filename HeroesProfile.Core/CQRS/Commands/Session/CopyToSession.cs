using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.CQRS.Notifications;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;

using MediatR;

using Polly;

namespace HeroesProfile.Core.CQRS.Commands.Session;


public static class CopyToSession
{
    public record Command(string FileToCopy) : IRequest<Response>;

    public record Response(bool Success);

    public class Handler : IRequestHandler<Command, Response>
    {
        private readonly AppSettings appSettings;
        private readonly SessionRepository sessionRepository;
        private readonly IMediator mediator;

        public Handler(AppSettings appSettings, SessionRepository sessionRepository, IMediator mediator)
        {
            this.appSettings = appSettings;
            this.sessionRepository = sessionRepository;
            this.mediator = mediator;
        }

        public async Task<Response> Handle(Command command, CancellationToken cancellationToken)
        {
            var context = new Context();
            context.Add("Command", command);

            PolicyResult result = await Policy
                .Handle<IOException>()
                .WaitAndRetryAsync(5, (attempt) => TimeSpan.FromSeconds(1))
                .ExecuteAndCaptureAsync((context, token) => UpdateSessionFileAsync((Command)context["Command"], token), context, cancellationToken);

            if (result.Outcome == OutcomeType.Successful)
            {
                await mediator.Publish(new SessionUpdated.Notification(sessionRepository.SessionData));
            }

            return new Response(result.Outcome == OutcomeType.Successful);
        }

        private async Task UpdateSessionFileAsync(Command command, CancellationToken cancellationToken)
        {
            string extension = Path.GetExtension(command.FileToCopy).TrimStart('.');
            string fullName = Path.Combine(appSettings.ApplicationSessionDirectory, $"session.{extension}");

            if (File.Exists(fullName))
            {
                File.Delete(fullName);
            }
            else
            {
                File.Copy(command.FileToCopy, fullName, overwrite: true);
            }

            await sessionRepository.UpdateAsync(fullName, cancellationToken);
        }
    }
}