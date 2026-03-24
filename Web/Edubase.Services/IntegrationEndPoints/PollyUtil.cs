using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;
using Polly.Timeout;

namespace Edubase.Services.IntegrationEndPoints
{
    public static class PollyUtil
    {
        /// <summary>
        /// Creates a retry + timeout policy
        /// </summary>
        /// <param name="retryIntervals">Array of retry intervals (seconds).</param>
        /// <param name="settingsKey">key for timeout seconds.</param>
        /// <returns>
        /// A combined policy that retries faults and times out safely.
        /// </returns>
        public static IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy(TimeSpan[] retryIntervals, string settingsKey)
        {
            var timeoutPolicy = CreateTimeoutPolicy(settingsKey);

            if (retryIntervals is null || retryIntervals.Length == 0)
            {
                return timeoutPolicy;
            }

            // Retry on network faults and timeouts
            var retryPolicy = Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .Or<TimeoutRejectedException>()
                .OrResult(r =>
                    r.StatusCode == HttpStatusCode.RequestTimeout ||
                    (int)r.StatusCode == 429 ||
                    (int)r.StatusCode >= 500)
                .WaitAndRetryAsync(retryIntervals);

            return Policy.WrapAsync(retryPolicy, timeoutPolicy);
        }

        /// <summary>
        /// Creates a timeout policy that cancels requests exceeding the configured time limit.
        /// </summary>
        /// <param name="settingsKey">Configuration key for timeout duration in seconds.</param>
        /// <returns>An asynchronous timeout policy for HTTP calls.</returns>
        public static IAsyncPolicy<HttpResponseMessage> CreateTimeoutPolicy(string settingsKey)
        {
            if (!int.TryParse(ConfigurationManager.AppSettings[settingsKey], out var timeoutSeconds))
            {
                timeoutSeconds = 10;
            }

            return Policy.TimeoutAsync<HttpResponseMessage>(
                TimeSpan.FromSeconds(timeoutSeconds),
                TimeoutStrategy.Pessimistic);
        }

        /// <summary>
        /// Converts a comma-separated string of seconds into an array of TimeSpans.
        /// The string should only contain non-negative integer values.
        /// <list type="bullet">
        ///     <item>Where non-integer or negative integer values are found, they are ignored.</item>
        ///     <item>Note that this may result in an empty array.</item>
        /// </list>
        /// </summary>
        /// <param name="csvSeconds">Comma-separated string of seconds.</param>
        /// <returns>An array of TimeSpans converted from the provided string.</returns>
        public static TimeSpan[] CsvSecondsToTimeSpans(string csvSeconds)
        {
            return csvSeconds.Split(',')
                .Select(x => x.Trim())
                .Where(x => int.TryParse(x, out var s) && s >= 0)
                .Select(x => TimeSpan.FromSeconds(int.Parse(x)))
                .ToArray();
        }
    }
}
