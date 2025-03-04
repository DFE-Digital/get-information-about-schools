using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Edubase.Web.UI.Models.News;
using Xunit;

namespace Edubase.Web.UIUnitTests.Areas.News
{
    public class NewsArticleViewModelTests
    {
        private List<ValidationResult> ValidateModel(object model)
        {
            var validationContext = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(model, validationContext, results, true);
            return results;
        }

        [Theory]
        [InlineData(null, "Day is required.")]
        [InlineData(0, "Day must be between 1 and 31.")]
        [InlineData(32, "Day must be between 1 and 31.")]
        public void Validate_ArticleDate_Day(int? day, string expectedError)
        {
            var model = new NewsArticleViewModel
            {
                ArticleDate_Day = day,
                ArticleDate_Month = 1,
                ArticleDate_Year = 2024,
                ArticleDate_Hour = 10,
                ArticleDate_Minute = 30
            };

            var results = ValidateModel(model);
            Assert.Contains(results, r => r.ErrorMessage == expectedError);
        }

        [Theory]
        [InlineData(null, "Month is required.")]
        [InlineData(0, "Month must be between 1 and 12.")]
        [InlineData(13, "Month must be between 1 and 12.")]
        public void Validate_ArticleDate_Month(int? month, string expectedError)
        {
            var model = new NewsArticleViewModel
            {
                ArticleDate_Day = 10,
                ArticleDate_Month = month,
                ArticleDate_Year = 2024,
                ArticleDate_Hour = 10,
                ArticleDate_Minute = 30
            };

            var results = ValidateModel(model);
            Assert.Contains(results, r => r.ErrorMessage == expectedError);
        }

        [Theory]
        [InlineData(null, "Year is required.")]
        [InlineData(1999, "Year must be between 2000 and 2100.")]
        [InlineData(2101, "Year must be between 2000 and 2100.")]
        public void Validate_ArticleDate_Year(int? year, string expectedError)
        {
            var model = new NewsArticleViewModel
            {
                ArticleDate_Day = 10,
                ArticleDate_Month = 1,
                ArticleDate_Year = year,
                ArticleDate_Hour = 10,
                ArticleDate_Minute = 30
            };

            var results = ValidateModel(model);
            Assert.Contains(results, r => r.ErrorMessage == expectedError);
        }

        [Theory]
        [InlineData(null, "Hour is required.")]
        [InlineData(-1, "Hour must be between 0 and 23.")]
        [InlineData(24, "Hour must be between 0 and 23.")]
        public void Validate_ArticleDate_Hour(int? hour, string expectedError)
        {
            var model = new NewsArticleViewModel
            {
                ArticleDate_Day = 10,
                ArticleDate_Month = 1,
                ArticleDate_Year = 2024,
                ArticleDate_Hour = hour,
                ArticleDate_Minute = 30
            };

            var results = ValidateModel(model);
            Assert.Contains(results, r => r.ErrorMessage == expectedError);
        }

        [Theory]
        [InlineData(null, "Minute is required.")]
        [InlineData(-1, "Minute must be between 0 and 59.")]
        [InlineData(60, "Minute must be between 0 and 59.")]
        public void Validate_ArticleDate_Minute(int? minute, string expectedError)
        {
            var model = new NewsArticleViewModel
            {
                ArticleDate_Day = 10,
                ArticleDate_Month = 1,
                ArticleDate_Year = 2024,
                ArticleDate_Hour = 10,
                ArticleDate_Minute = minute
            };

            var results = ValidateModel(model);
            Assert.Contains(results, r => r.ErrorMessage == expectedError);
        }

        [Fact]
        public void Validate_LeapYear_Feb29_Valid()
        {
            var model = new NewsArticleViewModel
            {
                ArticleDate_Day = 29,
                ArticleDate_Month = 2,
                ArticleDate_Year = 2024,
                ArticleDate_Hour = 10,
                ArticleDate_Minute = 30
            };

            var results = model.Validate(new ValidationContext(model)).ToList();
            Assert.Empty(results);
        }

        [Fact]
        public void Validate_NonLeapYear_Feb29_Invalid()
        {
            var model = new NewsArticleViewModel
            {
                ArticleDate_Day = 29,
                ArticleDate_Month = 2,
                ArticleDate_Year = 2023,
                ArticleDate_Hour = 10,
                ArticleDate_Minute = 30
            };

            var results = model.Validate(new ValidationContext(model)).ToList();

            Assert.NotEmpty(results);
            Assert.Contains(results, r => r.ErrorMessage == "The date specified is not valid.");
        }
    }
}
