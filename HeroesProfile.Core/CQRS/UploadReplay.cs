using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using HeroesProfile.Core.Models;
using HeroesProfile.Core.Services.Upload;

using MediatR;

namespace HeroesProfile.Core.CQRS
{
    public static class UploadReplay
    {
        public record Request(ReplayParseData Data, bool HotsApi = false, bool HotsLogs = false) : IRequest<Response>;

        public record Response(ReplayUploader.UploadResponse UploadResponse);

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly ReplayUploader.IClient uploadClient;

            public Handler(ReplayUploader.IClient uploadClient)
            {
                this.uploadClient = uploadClient;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                if (request.HotsApi)
                {
                    await uploadClient.UploadToHotsApiAsync(new ReplayUploader.UploadRequest(request.Data), cancellationToken);
                }

                if (request.HotsLogs)
                {
                    await uploadClient.UploadToHeroesProfileAsync(new ReplayUploader.UploadRequest(request.Data), cancellationToken);
                }

                return new Response(await uploadClient.UploadToHeroesProfileAsync(new ReplayUploader.UploadRequest(request.Data), cancellationToken));
            }
        }
    }
}