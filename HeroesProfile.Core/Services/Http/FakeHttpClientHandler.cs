using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HeroesProfile.Core.Services.Http
{
    public class FakeHttpClientHandler : DelegatingHandler
    {
        private readonly Dictionary<Uri, HttpResponseMessage> fakeResponses = new Dictionary<Uri, HttpResponseMessage>();

        private readonly ILogger<FakeHttpClientHandler> logger;

        public FakeHttpClientHandler(ILogger<FakeHttpClientHandler> logger)
        {
            this.logger = logger;
        }

        public void AddFakeResponse(Uri uri, HttpResponseMessage responseMessage)
        {
            fakeResponses.Add(uri, responseMessage);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Fake file uploads
            if (request.RequestUri.PathAndQuery.Contains("upload?fingerprint="))
            {
                return Task.FromResult(new HttpResponseMessage() { StatusCode = HttpStatusCode.OK, Content = new StringContent(@"{ ""replayID"": 0, ""success"": true, ""status"": ""Success"" }") });
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}
