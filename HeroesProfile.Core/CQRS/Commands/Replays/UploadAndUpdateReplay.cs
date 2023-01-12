using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.Clients;
using HeroesProfile.Core.CQRS.Queries;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;

using MediatR;

namespace HeroesProfile.Core.CQRS.Commands.Replays;

public static class UploadAndUpdateReplay
{
    public record Command(StoredReplay StoredReplay) : IRequest<Response>;

    public record Response(bool Success, int? ReplayId, UploadStatus UploadStatus);

    public class Handler : IRequestHandler<Command, Response>
    {
        private readonly IUploadClient uploadUploadClient;
        private readonly IMediator mediator;
        private readonly SessionRepository sessionRepository;
        private readonly AppSettings appSettings;
        private readonly UserSettingsRepository userSettingsRepository;

        public Handler(IUploadClient uploadUploadClient, IMediator mediator, SessionRepository sessionRepository, AppSettings appSettings, UserSettingsRepository userSettingsRepository)
        {
            this.uploadUploadClient = uploadUploadClient;
            this.mediator = mediator;
            this.sessionRepository = sessionRepository;
            this.appSettings = appSettings;
            this.userSettingsRepository = userSettingsRepository;
        }

        public async Task<Response> Handle(Command command, CancellationToken cancellationToken)
        {
            // Load the Replay for Upload (bytes to send)
            GetParsedReplay.Response response = await mediator.Send(new GetParsedReplay.Query(new FileInfo(command.StoredReplay.Path), options: null), cancellationToken);

            if (response.Data.Bytes == null || string.IsNullOrEmpty(response.Data.Fingerprint))
                return new(false, null, UploadStatus.UploadError);

            // Upload the Replay
            if (false)
            {
                await uploadUploadClient.UploadToHotsApiAsync(response.Data.Bytes, response.Data.Fingerprint, cancellationToken);
            }

            if (false)
            {
                await uploadUploadClient.UploadToHeroesProfileAsync(response.Data.Bytes, response.Data.Fingerprint, cancellationToken);
            }

            UploadResponse uploadResponse = await uploadUploadClient.UploadToHeroesProfileAsync(response.Data.Bytes, response.Data.Fingerprint, cancellationToken);

            UploadStatus uploadStatus = uploadResponse.Status;
            StoredReplay storedReplay = command.StoredReplay;
            int? replayId = uploadResponse.ReplayId;

            if (uploadResponse.Success)
            {
                storedReplay.ProcessStatus = ProcessStatus.Success;
            }
            else if (uploadResponse.Status == UploadStatus.Duplicate)
            {
                storedReplay.ProcessStatus = ProcessStatus.Duplicate;
            }
            else if (uploadResponse.Status == UploadStatus.UploadError)
            {
                storedReplay.ProcessStatus = ProcessStatus.Error;
            }
            else if (new[] { UploadStatus.AiDetected, UploadStatus.PtrRegion, UploadStatus.TooOld, UploadStatus.CustomGame, UploadStatus.Incomplete }.Contains(uploadResponse.Status))
            {
                storedReplay.ProcessStatus = ProcessStatus.NotSupported;
            }

            storedReplay.Updated = DateTime.UtcNow;
            storedReplay.UploadStatus = uploadStatus;
            storedReplay.ReplayId = replayId;

            await mediator.Send(new UpdateReplays.Command(storedReplay));


            return new(uploadResponse.Success, replayId, uploadStatus);
        }
    }
}