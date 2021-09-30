using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using MauiApp2.Core.Clients;

using MauiApp2.Core.Models;

using Microsoft.Extensions.Logging;

namespace MauiApp2.Core.Fakes
{
    public class FakeHeroesProfileDelegatingHandler : DelegatingHandler
    {
        private readonly Dictionary<Uri, HttpResponseMessage> fakeResponses = new();
        private readonly AppSettings appSettings;
        private readonly ILogger<FakeHeroesProfileDelegatingHandler> logger;
        private readonly Random random = new();

        private HashSet<int> Uploaded { get; } = new();

        public FakeHeroesProfileDelegatingHandler(AppSettings appSettings, ILogger<FakeHeroesProfileDelegatingHandler> logger)
        {
            this.appSettings = appSettings;
            this.logger = logger;
        }

        public void AddFakeResponse(Uri uri, HttpResponseMessage responseMessage)
        {
            fakeResponses.Add(uri, responseMessage);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (TalentsClient.ValidateUri.Equals(request.RequestUri.GetComponents(UriComponents.Path, UriFormat.Unescaped)))
            {
                if (random.Next(1, 3) == 1)
                {
                    return Task.FromResult(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(DateTime.Now.Millisecond.ToString()) });
                }
                else
                {
                    return Task.FromResult(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("Authentication failed") });
                }
            }

            // Simulate API request throttling
            // 25% chance to trigger 429
            if (random.Next(1, 8) == 1)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.TooManyRequests)
                {
                    Content = new StringContent("Slow down"),
                    Headers =
                    {
                        RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(2))
                    }
                });
            }

            // Fake PreMatch Id
            if (new Uri(appSettings.HeroesProfileUri, PreMatchClient.PreMatchUri).Equals(request.RequestUri))
            {
                return Task.FromResult(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(DateTime.Now.Millisecond.ToString()) });
            }

            // Fake Save Replay
            if (new Uri(appSettings.HeroesProfileApiUri, TalentsClient.SaveReplayUri).Equals(request.RequestUri))
            {
                return Task.FromResult(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(DateTime.Now.Millisecond.ToString()) });
            }

            // Fake file uploads
            if (request.RequestUri.LocalPath.Equals("/upload"))
            {
                int replayID = random.Next(1, 1000);

                UploadStatus status;

                if (Uploaded.Contains(replayID))
                {
                    status = UploadStatus.Duplicate;
                }
                else
                {
                    if (random.Next(1, 4) <= 2) // 75% chance its success
                    {
                        status = UploadStatus.Success;
                    }
                    else
                    {
                        status = Enum.GetValues(typeof(UploadStatus)).OfType<UploadStatus>().OrderBy(x => Guid.NewGuid()).First();
                    }


                    Uploaded.Add(replayID);
                }

                return Task.FromResult(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent($@"{{""replayID"": {replayID}, ""success"": {(status == UploadStatus.Success).ToString().ToLower()}, ""status"": ""{status}"" }}") });
            }

            // Fake the success of everything other request
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}
