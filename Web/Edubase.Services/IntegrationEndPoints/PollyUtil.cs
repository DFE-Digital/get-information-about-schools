using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;

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
        public static Policy CreateRetryPolicy(TimeSpan[] retryIntervals, string settingsKey)
        {

            if(retryIntervals is null || retryIntervals.Length == 0)
            {
                return Policy.NoOp();
            }

            var retryPolicy = CreateRetryPolicyInternal(retryIntervals);
            var timeoutPolicy = CreateTimeoutPolicy(settingsKey);

            return Policy.WrapAsync(retryPolicy, timeoutPolicy);
        }

        public static Policy CreateRetryPolicyInternal(TimeSpan[] retryIntervals)
        {

            return Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .WaitAndRetryAsync(retryIntervals);

        }

        public static Policy CreateTimeoutPolicy(string settingsKey)
        {
            var timeoutSettings = 10;
            switch (settingsKey)
            {
                case "AzureMapServiceTimeoutKey":
                    if (!int.TryParse(ConfigurationManager.AppSettings["AzureMapServiceTimeoutKey"], out timeoutSettings))
                    {
                        timeoutSettings = 10;
                    }
                    break;
                case "OSPlacesApiServicesTimeoutKey":
                    if (!int.TryParse(ConfigurationManager.AppSettings["OSPlacesApiServicesTimeoutKey"], out timeoutSettings))
                    {
                        timeoutSettings = 10;
                    }
                    break;
            }

            return Policy.TimeoutAsync(TimeSpan.FromSeconds(timeoutSettings));
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
