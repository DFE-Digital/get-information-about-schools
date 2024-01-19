using System;
using Xunit;
using Edubase.Services.IntegrationEndPoints;
using Polly.NoOp;
using Polly.Wrap;
using System.Configuration;
using Polly.Timeout;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

namespace Edubase.ServicesUnitTests.IntegrationEndPoints
{
    public class PollyUtilTests
    {
        [Fact]
        public void CreateRetryPolicy_ReturnsNoOpPolicy_WhenNullIntervalsPassedIn()
        {
            var policy = PollyUtil.CreateRetryPolicy(null,"");
            Assert.IsType<NoOpPolicy>(policy);
        }

        [Fact]
        public void CreateRetryPolicy_ReturnsNoOpPolicy_WhenEmptyIntervalsPassedIn()
        {
            var policy = PollyUtil.CreateRetryPolicy(new TimeSpan[0],"");
            Assert.IsType<NoOpPolicy>(policy);
        }

        [Fact]
        public void CreateRetryPolicy_ReturnsPolicyWrap_WhenIntervalsPassedIn()
        {
            var retryIntervals = new[] { TimeSpan.FromSeconds(1) };
            var settingsKey = "AzureMapService_Timeout";

            var policy = PollyUtil.CreateRetryPolicy(retryIntervals, settingsKey);

            Assert.NotNull(policy);
            Assert.IsType<PolicyWrap>(policy);
        }

        //Note: Polly doesn't expose the timeout settings once the policy is created

        [Fact]
        public async void CreateTimeoutPolicy_ReturnsCorrectTimeoutForAzureMapService()
        {
            var validKey = "AzureMapService_Timeout";
            ConfigurationManager.AppSettings[validKey] = "5";

            var policy = PollyUtil.CreateTimeoutPolicy(validKey);

            var sw = Stopwatch.StartNew();
            await Assert.ThrowsAsync<TimeoutRejectedException>(async () =>
            {
                await policy.ExecuteAsync(async (ct) =>
                {
                    await Task.Delay(6000, ct);
                }, CancellationToken.None);
            });
            sw.Stop();

            Assert.NotNull(policy);
            Assert.True(sw.Elapsed.Seconds >= 5 && sw.Elapsed.Seconds < 7, $"Timeout expected Elapsed >= 5 && Elapsed < 7 Actual: {sw.Elapsed.Seconds}");
        }


        [Fact]
        public async void CreateRetryPolicy_DefaultsTo10Seconds()
        {
            var invalidKey = "InvalidAzureMapService_Timeout";
            ConfigurationManager.AppSettings[invalidKey] = "invalid";

            var policy = PollyUtil.CreateTimeoutPolicy(invalidKey);

            var sw = Stopwatch.StartNew();
            Exception thrownException = null;
                try
                {
                    await policy.ExecuteAsync(async (ct) =>
                    {
                        await Task.Delay(11000, ct);
                    }, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    thrownException = ex;
                }
            
            sw.Stop();

            Assert.NotNull(policy);
            Assert.IsType<TimeoutRejectedException>(thrownException);
            Assert.True(sw.Elapsed.Seconds >= 10 && sw.Elapsed.Seconds < 12, $"Timeout expected Elapsed >= 10 && Elapsed < 12 Actual: {sw.Elapsed.Seconds}");
        }



        [Fact]
        public void CsvSecondsToTimeSpans_ReturnsCorrectTimeSpans()
        {
            var csvSeconds = "1,2,2,3,4,0";
            var timeSpans = PollyUtil.CsvSecondsToTimeSpans(csvSeconds);

            Assert.Equal(6, timeSpans.Length);

            Assert.Equal(TimeSpan.FromSeconds(1), timeSpans[0]);
            Assert.Equal(TimeSpan.FromSeconds(2), timeSpans[1]);
            Assert.Equal(TimeSpan.FromSeconds(2), timeSpans[2]);
            Assert.Equal(TimeSpan.FromSeconds(3), timeSpans[3]);
            Assert.Equal(TimeSpan.FromSeconds(4), timeSpans[4]);
            Assert.Equal(TimeSpan.FromSeconds(0), timeSpans[5]);
        }

        [Fact]
        public void CsvSecondsToTimeSpans_SingleNumberIsAcceptedAsValidInput()
        {
            var csvSeconds = "100";
            var timeSpans = PollyUtil.CsvSecondsToTimeSpans(csvSeconds);

            Assert.Single(timeSpans);

            Assert.Equal(TimeSpan.FromSeconds(100), timeSpans[0]);
        }

        [Fact]
        public void CsvSecondsToTimeSpans_CommaSeparatedNumbersAreAcceptedAsValidInput()
        {
            var csvSeconds = "100,101";
            var timeSpans = PollyUtil.CsvSecondsToTimeSpans(csvSeconds);

            Assert.Equal(2, timeSpans.Length);

            Assert.Equal(TimeSpan.FromSeconds(100), timeSpans[0]);
            Assert.Equal(TimeSpan.FromSeconds(101), timeSpans[1]);
        }

        [Fact]
        public void CsvSecondsToTimeSpans_ZeroIsAcceptedAsValidInput()
        {
            var csvSeconds = "0";
            var timeSpans = PollyUtil.CsvSecondsToTimeSpans(csvSeconds);

            Assert.Single(timeSpans);

            Assert.Equal(TimeSpan.FromSeconds(0), timeSpans[0]);
        }


        [Theory]
        [InlineData("", 0)]
        [InlineData("abc", 0)]
        [InlineData("abc,1,2", 2)]
        [InlineData("1,abc,2", 2)]
        [InlineData("1,2,abc", 2)]
        [InlineData("1,2,,3", 3)]
        [InlineData("1,2,*@&#,3", 3)]
        public void CsvSecondsToTimeSpans_WhenNonNumericInputs_InvalidInputsFilteredFromOutputTimeSpans(
            string csvSeconds, int expectedCount)
        {
            var timeSpans = PollyUtil.CsvSecondsToTimeSpans(csvSeconds);
            Assert.Equal(expectedCount, timeSpans.Length);
        }

        [Theory]
        [InlineData("-1", 0)]
        [InlineData("0,-1", 1)]
        [InlineData("-1,2", 1)]
        [InlineData("1,-2", 1)]
        [InlineData("-1,-2", 0)]
        [InlineData("1,-2,3", 2)]
        [InlineData("-1,-2,-3", 0)]
        public void CsvSecondsToTimeSpans_WhenNegativeInputs_InvalidInputsFilteredFromOutputTimeSpans(
            string csvSeconds, int expectedCount)
        {
            var timeSpans = PollyUtil.CsvSecondsToTimeSpans(csvSeconds);
            Assert.Equal(expectedCount, timeSpans.Length);
        }
    }
}
