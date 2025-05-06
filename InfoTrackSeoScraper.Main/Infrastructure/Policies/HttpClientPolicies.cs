using Polly;
using Polly.Extensions.Http;
using System;
using System.Net;
using System.Net.Http;

namespace InfoTrackSeoScraper.Main.Infrastructure.Policies
{
    /// <summary>
    /// Provides resilience policies for HTTP clients.
    /// </summary>
    public static class HttpClientPolicies
    {
        /// <summary>
        /// Creates a retry policy for transient HTTP errors and rate limiting.
        /// </summary>
        /// <remarks>
        /// Uses exponential backoff with 3 retry attempts.
        /// </remarks>
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}