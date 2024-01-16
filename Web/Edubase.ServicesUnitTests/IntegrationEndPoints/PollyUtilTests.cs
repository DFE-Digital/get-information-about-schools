using System;
using Xunit;
using Edubase.Services.IntegrationEndPoints;
using Polly.NoOp;
using Polly.Retry;
using Polly;

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
        public void CreateRetryPolicy_ReturnsRetryPolicy_WhenIntervalsPassedIn()
        {
            TimeSpan[] retryIntervals = new[] { TimeSpan.FromSeconds(1) };
            string settingsKey = "AzureMapServiceTimeoutKey";


            var policy = PollyUtil.CreateRetryPolicy(retryIntervals, settingsKey);
            Assert.NotNull(policy);
            Assert.IsAssignableFrom<Policy>(policy);
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
