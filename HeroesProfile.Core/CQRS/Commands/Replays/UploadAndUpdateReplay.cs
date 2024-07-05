using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Heroes.StormReplayParser;
using HeroesProfile.Core.Clients;
using HeroesProfile.Core.CQRS.Queries;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Repositories;
using MediatR;

namespace HeroesProfile.Core.CQRS.Commands.Replays;

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

            byte[] bytes = await File.ReadAllBytesAsync(response.Data.File.FullName, cancellationToken);

            UploadResponse uploadResponse = await uploadUploadClient.UploadToHeroesProfileAsync(bytes, response.Data.Fingerprint!, cancellationToken);
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

            storedReplay.Updated = DateTime.UtcNow;
            storedReplay.ReplayId = replayId;

            await mediator.Send(new UpdateReplays.Command([storedReplay]), cancellationToken);

            return new(uploadResponse.Success, replayId, storedReplay.ProcessStatus, ParseStatus: response.Data.ParseStatus);
        }
    }
}