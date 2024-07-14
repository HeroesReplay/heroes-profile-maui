using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using HeroesProfile.UI.Core.Models;
using Microsoft.Extensions.Logging;

namespace HeroesProfile.UI.Core.Clients;

public interface IUploadClient
{
    Task<List<string>> CheckDuplicatesAsync(List<string> fingerprints, CancellationToken cancellationToken);
    Task<UploadResponse> UploadToHeroesProfileAsync(StoredReplay replay, string fingerprint, CancellationToken cancellationToken);
}

public class UploadResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; } = false;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    [JsonPropertyName("status")]
    public UploadStatus Status { get; set; } = UploadStatus.UploadError;

    [JsonPropertyName("replayID")]
    public long? ReplayId { get; set; }
}

public class UploadClient(ILogger<UploadClient> logger, HttpClient httpClient) : IUploadClient
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
                    return await response.Content.ReadFromJsonAsync<UploadResponse>(cancellationToken)!;
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to upload replay to HeroesProfile");
        }

        
        return new UploadResponse()
        {
            Success = false,
            Status = UploadStatus.UploadError,
            ReplayId = null
        };
    }
}