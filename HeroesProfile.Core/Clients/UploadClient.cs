using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.Models;

namespace HeroesProfile.Core.Clients;


public interface IUploadClient
{
    Task<List<string>> CheckDuplicatesAsync(List<string> Fingerprints, CancellationToken cancellationToken);
    Task<UploadResponse> UploadToHeroesProfileAsync(byte[] data, string fingerprint, CancellationToken cancellationToken);
    Task<UploadResponse> UploadToHotsApiAsync(byte[] data, string fingerprint, CancellationToken cancellationToken); // We don't care other than a HTTP 200
    Task<UploadResponse> UploadToHotsLogsAsync(byte[] data, string fingerprint, CancellationToken cancellationToken); // We don't care other than a HTTP 200
}


public record UploadResponse(bool Success, UploadStatus Status, int? ReplayId = null);

public class UploadClient : IUploadClient
{
    private readonly AppSettings appSettings;
    private readonly HttpClient httpClient;

    public UploadClient(AppSettings appSettings, HttpClient httpClient)
    {
        this.appSettings = appSettings;
        this.httpClient = httpClient;
    }

    public async Task<List<string>> CheckDuplicatesAsync(List<string> Fingerprints, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<UploadResponse> UploadToHeroesProfileAsync(byte[] data, string fingerprint, CancellationToken cancellationToken)
    {
        using (ByteArrayContent fileContent = new ByteArrayContent(data))
        {
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");

            using (HttpResponseMessage response = await httpClient.PostAsync(new Uri($"upload?fingerprint={fingerprint}", UriKind.Relative), fileContent, cancellationToken))
            {
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync(cancellationToken);

                    using (var document = JsonDocument.Parse(json))
                    {
                        int replayId = document.RootElement.GetProperty("replayID").GetInt32();
                        bool success = document.RootElement.GetProperty("success").GetBoolean();
                        UploadStatus status = Enum.Parse<UploadStatus>(document.RootElement.GetProperty("status").GetString(), ignoreCase: true);

                        return new UploadResponse(Success: success, Status: status, ReplayId: replayId);
                    }
                }
            }
        }

        return new UploadResponse(Success: false, Status: UploadStatus.UploadError, ReplayId: null);
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