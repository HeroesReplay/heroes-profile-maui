using System;
using System.Net;
using System.Net.Http;

using Polly;
using Polly.Extensions.Http;

namespace HeroesProfile.Core
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/implement-http-call-retries-exponential-backoff-polly
    /// </summary>
    public static class PollyPolicies
    {
        private const string RetryAfterKey = "retry-after";

        public static IAsyncPolicy<HttpResponseMessage> GetHeroesProfileRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(responseMessage => responseMessage.StatusCode != HttpStatusCode.OK)
                .WaitAndRetryAsync(retryCount: 10, GetSleepDuration, OnRetry);
        }

        private static void OnRetry(DelegateResult<HttpResponseMessage> dr, TimeSpan timeSpan, int retryAttempt, Context context)
        {
            if (dr?.Result?.Headers?.RetryAfter != null && dr.Result.Headers.RetryAfter.Delta != null)
            {
                context[RetryAfterKey] = dr.Result.Headers.RetryAfter.Delta.Value;
            }
        }

        private static TimeSpan GetSleepDuration(int retryAttempt, Context context)
        {
            if (context.ContainsKey(RetryAfterKey))
            {
                return (TimeSpan)context[RetryAfterKey];
            }

            return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
        }
    }
}