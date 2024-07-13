using System.Net;
using System.Net.Http.Headers;
using HeroesProfile.UI.Core.Clients;
using HeroesProfile.UI.Core.Models;
using Microsoft.Extensions.Logging;

namespace HeroesProfile.UI.Core.Fakes;

public class FakeHeroesProfileDelegatingHandler(AppSettings appSettings, ILogger<FakeHeroesProfileDelegatingHandler> logger) : DelegatingHandler
{
    private readonly Dictionary<Uri, HttpResponseMessage> fakeResponses = new();

    private HashSet<long> Uploaded { get; } = new();

    public void AddFakeResponse(Uri uri, HttpResponseMessage responseMessage)
    {
        fakeResponses.Add(uri, responseMessage);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(Random.Shared.Next(0, 1)), cancellationToken);

        if (Random.Shared.Next(1, 8) == 1)
        {
            return new HttpResponseMessage(HttpStatusCode.TooManyRequests)
            {
                Content = new StringContent("Slow down"),
                Headers =
                {
                    RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(2))
                }
            };
        }

        // Fake PreMatch Id
        if (new Uri(appSettings.HeroesProfileUri, PreMatchClient.PreMatchUri).Equals(request.RequestUri))
        {
            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(DateTime.Now.Millisecond.ToString())
            };
        }

        // Fake file uploads
        if (request.RequestUri!.LocalPath.Equals("/upload"))
        {
            long replayId = Random.Shared.NextInt64(1, 1000);

            UploadStatus status;

            if (Uploaded.Contains(replayId))
            {
                status = UploadStatus.Duplicate;
            }
            else
            {
                if (Random.Shared.Next(1, 4) <= 2) // 75% chance its success
                {
                    status = UploadStatus.Success;
                }
                else
                {
                    status = Enum.GetValues(typeof(UploadStatus)).OfType<UploadStatus>().MinBy(x => Guid.NewGuid());
                }


                Uploaded.Add(replayId);
            }

            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(
                    $@"{{""replayID"": {replayId}, ""success"": {(status == UploadStatus.Success).ToString().ToLower()}, ""status"": ""{status}"" }}")
            };
        }

        return new HttpResponseMessage(HttpStatusCode.OK);
    }
}