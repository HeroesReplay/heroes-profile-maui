using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.CQRS.Queries;
using HeroesProfile.Core.Models;
using HeroesProfile.Core.Upload;
using MediatR;

namespace HeroesProfile.Core.CQRS.Commands
{
    public static class UploadAndUpdateReplay
    {
        public record Command(StoredReplay Replay, bool HotsApi = false, bool HotsLogs = false) : IRequest<Response>;

        public record Response(bool Success, int ReplayId, UploadStatus UploadStatus);

        public class Handler : IRequestHandler<Command, Response>
        {
            private readonly UploadReplay.IClient uploadClient;
            private readonly IMediator mediator;

            public Handler(UploadReplay.IClient uploadClient, IMediator mediator)
            {
                this.uploadClient = uploadClient;
                this.mediator = mediator;
            }

            public async Task<Response> Handle(Command command, CancellationToken cancellationToken)
            {
                // Load the Replay for Upload (bytes to send)
                GetParsedReplay.Response response = await mediator.Send(new GetParsedReplay.Query(new FileInfo(command.Replay.Path)), cancellationToken);

                // Upload the Replay
                if (command.HotsApi)
                {
                    await uploadClient.UploadToHotsApiAsync(response.Data.Bytes, response.Data.Fingerprint, cancellationToken);
                }

                if (command.HotsLogs)
                {
                    await uploadClient.UploadToHeroesProfileAsync(response.Data.Bytes, response.Data.Fingerprint, cancellationToken);
                }

                UploadReplay.UploadResponse uploadResponse = await uploadClient.UploadToHeroesProfileAsync(response.Data.Bytes, response.Data.Fingerprint, cancellationToken);

                // Update the Replay
                command.Replay.UploadStatus = uploadResponse.Status;

                if (uploadResponse.Success)
                {
                    command.Replay.ProcessStatus = ProcessStatus.Success;
                }
                else if (uploadResponse.Status == UploadStatus.Duplicate)
                {
                    command.Replay.ProcessStatus = ProcessStatus.Duplicate;
                }
                else if (uploadResponse.Status == UploadStatus.UploadError)
                {
                    command.Replay.ProcessStatus = ProcessStatus.Error;
                }
                else if (new[] { UploadStatus.AiDetected, UploadStatus.PtrRegion, UploadStatus.TooOld, UploadStatus.CustomGame, UploadStatus.Incomplete }.Contains(uploadResponse.Status))
                {
                    command.Replay.ProcessStatus = ProcessStatus.NotSupported;
                }

                await mediator.Send(new UpdateReplay.Command(command.Replay));

                return new(uploadResponse.Success, uploadResponse.ReplayId, uploadResponse.Status);
            }
        }
    }
}