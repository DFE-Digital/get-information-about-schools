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
        /// <param name="settingsKey">App.config key for timeout seconds.</param>
        /// <returns>
        /// A combined policy that retries faults and times out safely.
        /// </returns>
        public static IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy(TimeSpan[] retryIntervals, string settingsKey)
        {
            if (retryIntervals is null || retryIntervals.Length == 0)
            {
                return Policy.NoOpAsync<HttpResponseMessage>();
            }

            // Retry policy: handles transient network errors and TaskCanceledException
            var retryPolicy = Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .WaitAndRetryAsync(
                    retryIntervals,
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        // Optional low-severity logging hook
                    });

            var timeoutPolicy = CreateTimeoutPolicy(settingsKey);

            var combinedPolicy = Policy.WrapAsync(retryPolicy, timeoutPolicy);

            var safePolicy = Policy<HttpResponseMessage>
                .Handle<TimeoutRejectedException>()
                .FallbackAsync(
                    fallbackAction: (ct) =>
                    {
                        // Returns a safe 408 response instead of throwing
                        var response = new HttpResponseMessage(HttpStatusCode.RequestTimeout)
                        {
                            ReasonPhrase = "Request timed out by Polly policy"
                        };

                        // Optional logging (low severity)
                        return Task.FromResult(response);
                    });

            return safePolicy.WrapAsync(combinedPolicy);
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
                TimeoutStrategy.Optimistic,
                onTimeoutAsync: async (context, timeout, task, exception) =>
                {
                    // Optional: structured logging, low severity
                    await Task.CompletedTask;
                });
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
            var retryIntervals = csvSeconds.Split(',')
                .Select(x => x.Trim())
                .Where(x => int.TryParse(x, out var seconds) && seconds >= 0)
                .Select(x => TimeSpan.FromSeconds(int.Parse(x)))
                .ToArray();

            return retryIntervals;
        }
    }
}
