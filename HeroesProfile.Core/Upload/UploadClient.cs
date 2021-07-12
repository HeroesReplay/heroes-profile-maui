using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.Models;

namespace HeroesProfile.Core.Upload
{
    public static class UploadReplay
    {
        public interface IClient
        {
            Task<List<string>> CheckDuplicatesAsync(List<string> Fingerprints, CancellationToken cancellationToken);
            Task<UploadResponse> UploadToHeroesProfileAsync(byte[] data, string fingerprint, CancellationToken cancellationToken);
            Task<UploadResponse> UploadToHotsApiAsync(byte[] data, string fingerprint, CancellationToken cancellationToken); // We don't care other than a HTTP 200
            Task<UploadResponse> UploadToHotsLogsAsync(byte[] data, string fingerprint, CancellationToken cancellationToken); // We don't care other than a HTTP 200
        }


        public record UploadResponse(int ReplayId, bool Success, UploadStatus Status);

        public class Client : IClient
        {
            private readonly Settings settings;
            private readonly HttpMessageHandler httpMessageHandler;

            public Client(Settings settings, HttpMessageHandler httpMessageHandler)
            {
                this.settings = settings;
                this.httpMessageHandler = httpMessageHandler;
            }

            public async Task<List<string>> CheckDuplicatesAsync(List<string> Fingerprints, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            public async Task<UploadResponse> UploadToHeroesProfileAsync(byte[] data, string fingerprint, CancellationToken cancellationToken)
            {
                using (var httpClient = new HttpClient(httpMessageHandler))
                {
                    using (ByteArrayContent fileContent = new ByteArrayContent(data))
                    {
                        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");

                        using (HttpResponseMessage response = await httpClient.PostAsync($"{settings.HeroesProfileApiUrl}/upload?fingerprint={fingerprint}", fileContent, cancellationToken))
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

            public async Task<UploadResponse> UploadToHotsApiAsync(byte[] data, string fingerprint, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            public async Task<UploadResponse> UploadToHotsLogsAsync(byte[] data, string fingerprint, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}