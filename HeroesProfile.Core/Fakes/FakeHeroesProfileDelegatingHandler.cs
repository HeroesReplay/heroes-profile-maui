using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HeroesProfile.Core.Clients;
using Microsoft.Extensions.Logging;

namespace HeroesProfile.Core.Fakes
{
    public class FakeHeroesProfileDelegatingHandler : DelegatingHandler
    {
        private readonly Dictionary<Uri, HttpResponseMessage> fakeResponses = new Dictionary<Uri, HttpResponseMessage>();

        private readonly ILogger<FakeHeroesProfileDelegatingHandler> logger;

        public FakeHeroesProfileDelegatingHandler(ILogger<FakeHeroesProfileDelegatingHandler> logger)
        {
            this.logger = logger;
        }

        public void AddFakeResponse(Uri uri, HttpResponseMessage responseMessage)
        {
            fakeResponses.Add(uri, responseMessage);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            
            if (Uri.Compare(request.RequestUri, TalentsClient.SaveReplayUri, UriComponents.Path, UriFormat.Unescaped, StringComparison.OrdinalIgnoreCase) == 0)
            {
                return Task.FromResult(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(DateTime.Now.Millisecond.ToString()) });
            }

            // Fake file uploads
            if (request.RequestUri.PathAndQuery.Contains("upload?fingerprint="))
            {
                return Task.FromResult(new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(@"{ ""replayID"": 0, ""success"": true, ""status"": ""Success"" }") });
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}
