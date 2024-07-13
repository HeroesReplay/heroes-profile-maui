using System.Net.Http.Headers;
using System.Text.Json;
using HeroesProfile.UI.Core.Models;

namespace HeroesProfile.UI.Core.Clients;

public interface IUploadClient
{
    Task<List<string>> CheckDuplicatesAsync(List<string> fingerprints, CancellationToken cancellationToken);
    Task<UploadResponse> UploadToHeroesProfileAsync(StoredReplay replay, string fingerprint, CancellationToken cancellationToken);
}

public record UploadResponse(bool Success, UploadStatus Status, long? ReplayId = null);

public class UploadClient(AppSettings appSettings, HttpClient httpClient) : IUploadClient
{
    public async Task<List<string>> CheckDuplicatesAsync(List<string> fingerprints, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<UploadResponse> UploadToHeroesProfileAsync(StoredReplay replay, string fingerprint, CancellationToken cancellationToken)
    {
        await using (var filestream = new FileStream(replay.Path, FileMode.Open))
        {
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri($"upload?fingerprint={fingerprint}", UriKind.Relative));

            request.Content = new MultipartFormDataContent()
            {
                {
                    new StreamContent(filestream), "file", Path.GetFileName(replay.Path)
                }
            };

            var response = await httpClient.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync(cancellationToken);

                using (var document = JsonDocument.Parse(json))
                {
                    long replayId = document.RootElement.GetProperty("replayID").GetInt64();
                    bool success = document.RootElement.GetProperty("success").GetBoolean();
                    string status = document.RootElement.GetProperty("status").GetString();

                    return Enum.TryParse(status, ignoreCase: true, out UploadStatus uploadStatus)
                        ? new UploadResponse(Success: success, Status: uploadStatus, ReplayId: replayId)
                        : new UploadResponse(Success: success, Status: UploadStatus.UploadError, ReplayId: replayId);
                }
            }
        }

        return new UploadResponse(Success: false, Status: UploadStatus.UploadError, ReplayId: null);
    }
}