using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using HeroesProfile.Core.Clients;
using HeroesProfile.Core.Models;

using Microsoft.Extensions.Logging;

namespace HeroesProfile.Core.Fakes
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
            // Simulate API request throttling
            if (random.Next(1, 4) == 1)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.TooManyRequests)
                {
                    Headers =
                    {
                        RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(10))
                    }
                });
            }


            if (new Uri(appSettings.HeroesProfileApiUri, TalentsClient.SaveReplayUri).Equals(request.RequestUri))
            {
                return Task.FromResult(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(DateTime.Now.Millisecond.ToString()) });
            }

            // Fake file uploads
            if (request.RequestUri.LocalPath.Equals("/upload"))
            {
                int replayID = random.Next(1, 500);

                UploadStatus status = UploadStatus.InProgress;

                if (Uploaded.Contains(replayID))
                {
                    status = UploadStatus.Duplicate;
                }
                else
                {
                    status = Enum.GetValues(typeof(UploadStatus)).OfType<UploadStatus>().OrderBy(x => Guid.NewGuid()).First();
                    Uploaded.Add(replayID);
                }



                return Task.FromResult(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent($@"{{""replayID"": {replayID}, ""success"": {(status == UploadStatus.Success).ToString().ToLower()}, ""status"": ""{status}"" }}") });
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}
