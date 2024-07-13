using Heroes.StormReplayParser;
using HeroesProfile.UI.Core.Clients;
using HeroesProfile.UI.Core.CQRS.Queries;
using HeroesProfile.UI.Core.Models;
using HeroesProfile.UI.Core.Repositories;
using MediatR;

namespace HeroesProfile.UI.Core.CQRS.Commands.Replays;

public static class UploadAndUpdateReplay
{
    public record Command(StoredReplay StoredReplay) : IRequest<Response>;

    public record Response(bool Success, long? ReplayId, ProcessStatus Status, StormReplayParseStatus ParseStatus);

    public class Handler(
        IUploadClient uploadUploadClient,
        IMediator mediator,
        SessionRepository sessionRepository,
        AppSettings appSettings,
        UserSettingsRepository userSettingsRepository)
        : IRequestHandler<Command, Response>
    {
        private static readonly UploadStatus[] Unsupported =
        [
            UploadStatus.AiDetected,
            UploadStatus.PtrRegion,
            UploadStatus.TooOld,
            UploadStatus.CustomGame,
            UploadStatus.Incomplete
        ];

        public async Task<Response> Handle(Command command, CancellationToken cancellationToken)
        {
            var query = new GetParsedReplay.Query(new FileInfo(command.StoredReplay.Path), Options: ParseOptions.MinimalParsing);
            GetParsedReplay.Response response = await mediator.Send(query, cancellationToken);

            if (response.Data.ParseStatus != StormReplayParseStatus.Success)
            {
                return new(Success: false, ReplayId: null, ProcessStatus.Error, ParseStatus: response.Data.ParseStatus);
            }

            if (response.Data.ProcessStatus == ProcessStatus.Duplicate || response.Data.ProcessStatus == ProcessStatus.NotSupported)
            {
                return new(Success: false, ReplayId: null, response.Data.ProcessStatus, ParseStatus: response.Data.ParseStatus);
            }

            if (!response.Data.IsUploadable)
                throw new NotSupportedException("The state of the replay is not supported for upload.");

            UploadResponse uploadResponse = await uploadUploadClient.UploadToHeroesProfileAsync(command.StoredReplay, response.Data.Fingerprint!, cancellationToken);
            StoredReplay storedReplay = command.StoredReplay;
            long? replayId = uploadResponse.ReplayId;

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
            else if (Unsupported.Contains(uploadResponse.Status))
            {
                storedReplay.ProcessStatus = ProcessStatus.NotSupported;
            }

            storedReplay.Updated = DateTime.Now;
            storedReplay.ReplayId = replayId;

            await mediator.Send(new UpdateReplays.Command([storedReplay]), cancellationToken);

            return new(uploadResponse.Success, replayId, storedReplay.ProcessStatus, ParseStatus: response.Data.ParseStatus);
        }
    }
}