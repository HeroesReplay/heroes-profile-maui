using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.Models;

namespace HeroesProfile.Core.Services.Upload
{
    public static class ReplayUploader
    {
        public interface IClient
        {
            Task<CheckDuplicatesResponse> CheckDuplicatesAsync(CheckDuplicatesRequest request, CancellationToken cancellationToken);
            Task<UploadResponse> UploadToHeroesProfileAsync(UploadRequest request, CancellationToken cancellationToken);
            Task<UploadResponse> UploadToHotsApiAsync(UploadRequest request, CancellationToken cancellationToken); // We don't care other than a HTTP 200
            Task<UploadResponse> UploadToHotsLogsAsync(UploadRequest request, CancellationToken cancellationToken); // We don't care other than a HTTP 200
        }

        public record CheckDuplicatesRequest(List<ReplayParseData> Data);

        public record CheckDuplicatesResponse(List<string> Fingerprints);

        public record UploadResponse(int ReplayId, bool Success, UploadStatus Status);

        public record UploadRequest(ReplayParseData Data);

        public class Client : IClient
        {
            private readonly Settings settings;
            private readonly HttpMessageHandler httpMessageHandler;

            public Client(Settings settings, HttpMessageHandler httpMessageHandler)
            {
                this.settings = settings;
                this.httpMessageHandler = httpMessageHandler;
            }

            public async Task<CheckDuplicatesResponse> CheckDuplicatesAsync(CheckDuplicatesRequest request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            public async Task<UploadResponse> UploadToHeroesProfileAsync(UploadRequest request, CancellationToken cancellationToken)
            {
                using (var httpClient = new HttpClient(httpMessageHandler))
                {
                    using (ByteArrayContent fileContent = new ByteArrayContent(request.Data.Bytes))
                    {
                        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");

                        using (HttpResponseMessage response = await httpClient.PostAsync($"{settings.HeroesProfileApiUrl}/upload?fingerprint={request.Data.Fingerprint}", fileContent, cancellationToken))
                        {
                            if (response.IsSuccessStatusCode)
                            {
                                var json = await response.Content.ReadAsStringAsync(cancellationToken);

                                using (var document = JsonDocument.Parse(json))
                                {
                                    return new UploadResponse(document.RootElement.GetProperty("replayID").GetInt32(), document.RootElement.GetProperty("success").GetBoolean(), Enum.Parse<UploadStatus>(document.RootElement.GetProperty("status").GetString(), ignoreCase: true));
                                }
                            }
                        }
                    }
                }

                return new UploadResponse(0, false, UploadStatus.UploadError);
            }

            public async Task<UploadResponse> UploadToHotsApiAsync(UploadRequest request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            public async Task<UploadResponse> UploadToHotsLogsAsync(UploadRequest request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}