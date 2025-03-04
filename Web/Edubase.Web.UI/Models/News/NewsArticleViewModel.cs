using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Edubase.Data.Entity;

namespace Edubase.Web.UI.Models.News
{
    public enum eNewsArticleAction
    {
        Start,
        Review
    }

    public class NewsArticleViewModel : IValidatableObject
    {
        public string Id { get; set; }

        public eNewsArticleAction Action { get; set; }

        [Required(ErrorMessage = "The title is required.")]
        public string Title { get; set; }

        public DateTimeViewModel ArticleDate { get; set; } = new DateTimeViewModel();

        [Required(ErrorMessage = "Day is required.")]
        [Range(1, 31, ErrorMessage = "Day must be between 1 and 31.")]
        public int? ArticleDate_Day { get; set; }

        [Required(ErrorMessage = "Month is required.")]
        [Range(1, 12, ErrorMessage = "Month must be between 1 and 12.")]
        public int? ArticleDate_Month { get; set; }

        [Required(ErrorMessage = "Year is required.")]
        [Range(2000, 2100, ErrorMessage = "Year must be between 2000 and 2100.")]
        public int? ArticleDate_Year { get; set; }

        [Required(ErrorMessage = "Hour is required.")]
        [Range(0, 23, ErrorMessage = "Hour must be between 0 and 23.")]
        public int? ArticleDate_Hour { get; set; }

        [Required(ErrorMessage = "Minute is required.")]
        [Range(0, 59, ErrorMessage = "Minute must be between 0 and 59.")]
        public int? ArticleDate_Minute { get; set; }

        public bool ShowDate { get; set; } = true;

        [Required(ErrorMessage = "The content is required.")]
        [AllowHtml]
        public string Content { get; set; }

        public bool GoBack { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();

            if (!ArticleDate_Day.HasValue || !ArticleDate_Month.HasValue || !ArticleDate_Year.HasValue)
            {
                return errors;
            }

            if (!IsValidDate(ArticleDate_Day.Value, ArticleDate_Month.Value, ArticleDate_Year.Value))
            {
                errors.Add(new ValidationResult("The date specified is not valid.", new[] { "ArticleDate_Day" }));
            }
            return errors;
        }

        public NewsArticle ToArticle(NewsArticle originalArticle = null)
        {
            var newArticle = originalArticle ?? new NewsArticle();

            newArticle.Title = Title;
            newArticle.Content = Content;
            newArticle.ArticleDate = new DateTime(
                ArticleDate_Month ?? DateTime.Now.Month,
                ArticleDate_Year ?? DateTime.Now.Year,
                ArticleDate_Day ?? DateTime.Now.Day,
                ArticleDate_Hour ?? 0,
                ArticleDate_Minute ?? 0,
                0
            );

            newArticle.ShowDate = ShowDate;
            return newArticle;
        }

        public NewsArticleViewModel Set(NewsArticle article)
        {
            Id = article.RowKey;
            Title = article.Title;
            Content = article.Content;
            ShowDate = article.ShowDate;

            ArticleDate_Day = article.ArticleDate.Day;
            ArticleDate_Month = article.ArticleDate.Month;
            ArticleDate_Year = article.ArticleDate.Year;
            ArticleDate_Hour = article.ArticleDate.Hour;
            ArticleDate_Minute = article.ArticleDate.Minute;

            return this;
        }

        private bool IsValidDate(int day, int month, int year)
        {
            if (month < 1 || month > 12)
            {
                return false;
            }

            if (day < 1 || day > 31)
            {
                return false;
            }

            if (month == 2)
            {
                var isLeapYear = DateTime.IsLeapYear(year);
                var maxDaysInFeb = isLeapYear ? 29 : 28;
                if (day > maxDaysInFeb)
                {
                    return false;
                }
            }

            if ((month == 4 || month == 6 || month == 9 || month == 11) && day > 30)
            {
                return false;
            }
            return true;
        }

    }
}
