using System.Net.Http.Headers;
using System.Text.Json;
using HeroesProfile.UI.Core.Models;

namespace HeroesProfile.UI.Core.Clients;

public interface IUploadClient
{
    Task<List<string>> CheckDuplicatesAsync(List<string> fingerprints, CancellationToken cancellationToken);
    Task<UploadResponse> UploadToHeroesProfileAsync(byte[] data, string fingerprint, CancellationToken cancellationToken);
}

public record UploadResponse(bool Success, UploadStatus Status, long? ReplayId = null);

public class UploadClient(AppSettings appSettings, HttpClient httpClient) : IUploadClient
{
    public async Task<List<string>> CheckDuplicatesAsync(List<string> fingerprints, CancellationToken cancellationToken)
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
                        long replayId = document.RootElement.GetProperty("replayID").GetInt64();
                        bool success = document.RootElement.GetProperty("success").GetBoolean();
                        string status = document.RootElement.GetProperty("status").GetString();

                        return Enum.TryParse(status, ignoreCase: true, out UploadStatus uploadStatus)
                            ? new UploadResponse(Success: success, Status: uploadStatus, ReplayId: replayId)
                            : new UploadResponse(Success: success, Status: UploadStatus.UploadError, ReplayId: replayId);
                    }
                }
            }
        }

        return new UploadResponse(Success: false, Status: UploadStatus.UploadError, ReplayId: null);
    }
}