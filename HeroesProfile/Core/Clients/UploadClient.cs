using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using HeroesProfile.UI.Core.Models;
using Microsoft.Extensions.Logging;

namespace HeroesProfile.UI.Core.Clients;

public interface IUploadClient
{
    Task<List<string>> CheckDuplicatesAsync(List<string> fingerprints, CancellationToken cancellationToken);
    Task<UploadResponse> UploadToHeroesProfileAsync(StoredReplay replay, string fingerprint, CancellationToken cancellationToken);
}

public record UploadResponse(bool Success, UploadStatus Status, long? ReplayId = null);

public class UploadClient(ILogger<UploadClient> logger, AppSettings appSettings, HttpClient httpClient) : IUploadClient
{
    public async Task<List<string>> CheckDuplicatesAsync(List<string> fingerprints, CancellationToken cancellationToken)
    {
        using (var httpResponse = await httpClient.PostAsync(new Uri("api/replays/fingerprints"), new StringContent(string.Join('\n', fingerprints), Encoding.UTF8, "text/plain"), cancellationToken))
        {
            if (httpResponse.IsSuccessStatusCode)
            {
                var response = await httpResponse.Content.ReadAsStringAsync();
                return JsonDocument.Parse(response).RootElement.GetProperty("exists").EnumerateArray().Select(x => x.GetString()).ToList()!;
            }
        }

        return [];
    }

    public async Task<UploadResponse> UploadToHeroesProfileAsync(StoredReplay replay, string fingerprint, CancellationToken cancellationToken)
    {
        try
        {
            await using (var filestream = new FileStream(replay.Path, FileMode.Open))
            {
                var request = new HttpRequestMessage(HttpMethod.Post, new Uri($"api/upload/heroesprofile/desktop?fingerprint={fingerprint}&version=maui", UriKind.Relative))
                {
                    Content = new MultipartFormDataContent()
                    {
                        {
                            new StreamContent(filestream), "file", Path.GetFileName(replay.Path)
                        }
                    }
                };

                var response = await httpClient.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync(cancellationToken);

                    using (var document = JsonDocument.Parse(json))
                    {
                        long replayId = document.RootElement.GetProperty("replayID").GetInt64();
                        bool success = false;

                        if (document.RootElement.TryGetProperty("success", out var jsonElement))
                        {
                            success = jsonElement.GetBoolean();
                        }

                        string status = document.RootElement.GetProperty("status").GetString();

                        return Enum.TryParse(status, ignoreCase: true, out UploadStatus uploadStatus)
                            ? new UploadResponse(Success: success, Status: uploadStatus, ReplayId: replayId)
                            : new UploadResponse(Success: success, Status: UploadStatus.UploadError, ReplayId: replayId);
                    }
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to upload replay to HeroesProfile");
        }

        return new UploadResponse(Success: false, Status: UploadStatus.UploadError, ReplayId: null);
    }
}