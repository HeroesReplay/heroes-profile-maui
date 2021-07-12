using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using HeroesProfile.Core.CQRS;
using HeroesProfile.Core.Models;

using MediatR;

using Microsoft.Extensions.Hosting;

namespace HeroesProfile.Core.Services.Background
{
    public class StartupScanner : BackgroundService
    {
        private readonly IMediator mediator;

        private bool started;

        public StartupScanner(IMediator mediator)
        {
            this.mediator = mediator;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (started) return;
            started = true;

            ProcessReplays.Response processedResponse = await mediator.Send(new ProcessReplays.Request(), stoppingToken);

            // Now get stored replays that are NOT uploaded but are VALID

            var filters = new List<GetReplays.Filter>()
            {
                new GetReplays.Filter(ParseResult.Success, ProcessStatus.Pending), // stored and not yet uploaded
                new GetReplays.Filter(ParseResult.Success, ProcessStatus.Fail) // maybe it failed last time
            };

            GetReplays.Response replaysResponse = await mediator.Send(new GetReplays.Request(filters));

            foreach (StoredReplay item in replaysResponse.Replays)
            {
                if (item.ParseResult == ParseResult.Success && item.ProcessStatus == ProcessStatus.Pending || item.ProcessStatus == ProcessStatus.Fail)
                {
                    GetParsedReplay.Response parsedResponse = await mediator.Send(new GetParsedReplay.Request(new FileInfo(item.Path)));

                    UploadReplay.Response uploadResponse = await mediator.Send(new UploadReplay.Request(parsedResponse.Data, HotsApi: false, HotsLogs: false), stoppingToken);

                    item.UploadStatus = uploadResponse.UploadResponse.Status;

                    if (uploadResponse.UploadResponse.Success)
                    {
                        item.ProcessStatus = ProcessStatus.Success;
                    }
                    else if (uploadResponse.UploadResponse.Status == UploadStatus.Duplicate)
                    {
                        item.ProcessStatus = ProcessStatus.Duplicate;
                    }
                    else if (uploadResponse.UploadResponse.Status == UploadStatus.UploadError)
                    {
                        item.ProcessStatus = ProcessStatus.Fail;
                    }
                    else if (new[] { UploadStatus.AiDetected, UploadStatus.PtrRegion, UploadStatus.TooOld, UploadStatus.CustomGame, UploadStatus.Incomplete }.Contains(uploadResponse.UploadResponse.Status))
                    {
                        item.ProcessStatus = ProcessStatus.NotSupported;
                    }

                    await mediator.Send(new UpdateReplay.Request(item), stoppingToken);
                }
            }
        }
    }
}