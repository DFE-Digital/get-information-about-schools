using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.Timeout;

namespace Edubase.Services.IntegrationEndPoints
{
    public static class PollyUtil
    {
        /// <summary>
        /// Creates a retry policy based on the specified retry intervals.
        /// </summary>
        /// <param name="retryIntervals">The array of time intervals to wait between retries.</param>
        /// <returns>
        ///     A retry policy that handles <see cref="HttpRequestException"/> and waits for the specified retry intervals.
        ///     If <paramref name="retryIntervals"/> is null or empty, returns a no-op policy that doesn't perform any retries.
        /// </returns>
        public static IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy(TimeSpan[] retryIntervals, string settingsKey)
        {
            if(retryIntervals is null || retryIntervals.Length == 0)
            {
                return Policy.NoOpAsync<HttpResponseMessage>();
            }

            var retryPolicy = Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .WaitAndRetryAsync(retryIntervals, onRetry: (outcome, TimeSpan, retryAttempt, context) =>
                {
                    Console.WriteLine($"retry attempt {retryAttempt} due to: {outcome.Exception?.Message}");
                });

            var timeoutPolicy = CreateTimeoutPolicy(settingsKey);

            return Policy.WrapAsync(retryPolicy, timeoutPolicy);
        }

        public static IAsyncPolicy<HttpResponseMessage> CreateTimeoutPolicy(string settingsKey)
        {
            if (!int.TryParse(ConfigurationManager.AppSettings[settingsKey], out var timeoutSettings))
            {
                timeoutSettings = 10;
            }

            var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(
                TimeSpan.FromSeconds(timeoutSettings),
                TimeoutStrategy.Pessimistic,
                onTimeoutAsync: async (context, timeout, task, exception) =>
                {
                    Console.WriteLine($"timed out after {timeout.TotalSeconds} seconds - Exception {exception.Message}");
                    await Task.CompletedTask;
                });

            var fallbackPolicy = Policy<HttpResponseMessage>
                .Handle<TimeoutRejectedException>()
                .FallbackAsync(
                    fallbackAction: (context, cancellationToken) =>
                    {
                        var currentCount = TimeoutTracker.IncrementTimeoutCount();

                        if (currentCount >= 10)
                        {
                            return Task.FromException<HttpResponseMessage>(
                            new TaskCanceledException("Operation canceled for repeated timeouts - Polly"));
                        }
                        else
                        {
                            return Task.FromException<HttpResponseMessage>(
                                new TimeoutRejectedException());
                        }
                    },
                    async (exception, context) =>
                    {
                        await Task.CompletedTask;
                    });

            var policyWrap = Policy.WrapAsync(fallbackPolicy, timeoutPolicy);

            return policyWrap;
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
