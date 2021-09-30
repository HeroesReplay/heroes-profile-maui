using System;
using System.Net;
using System.Net.Http;

using Microsoft.Extensions.Logging;

using Polly;
using Polly.Extensions.Http;

namespace MauiApp2.Core
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/implement-http-call-retries-exponential-backoff-polly
    /// </summary>
    public static class PollyPolicies
    {
        private const string RetryAfterKey = "retry-after";

        public static IAsyncPolicy<HttpResponseMessage> GetHeroesProfileRetryPolicy<T>(ILogger<T> logger)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(responseMessage => responseMessage.StatusCode != HttpStatusCode.OK)
                .WaitAndRetryAsync(
                    retryCount: 10,
                    sleepDurationProvider: (retryAttempt, context) => GetSleepDuration(retryAttempt, context, logger),
                    onRetry: (dr, ts, retryAttempt, ctx) => OnRetry(dr, ts, retryAttempt, ctx, logger));
        }

        private static void OnRetry<T>(DelegateResult<HttpResponseMessage> dr, TimeSpan timeSpan, int retryAttempt, Context context, ILogger<T> logger)
        {
            if (dr?.Result?.Headers?.RetryAfter != null && dr.Result.Headers.RetryAfter.Delta != null)
            {
                logger.LogInformation($"Setting RetryAfter: {dr.Result.Headers.RetryAfter.Delta.Value}");

                context[RetryAfterKey] = dr.Result.Headers.RetryAfter.Delta.Value;
            }
        }

        private static TimeSpan GetSleepDuration<T>(int retryAttempt, Context context, ILogger<T> logger)
        {
            if (context.ContainsKey(RetryAfterKey))
            {
                return (TimeSpan)context[RetryAfterKey];
            }

            return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
        }
    }
}