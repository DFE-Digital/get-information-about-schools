using System;
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

    public class NewsArticleViewModel
    {
        public string Id { get; set; }

        public eNewsArticleAction Action { get; set; }

        [Required]
        public string Title { get; set; }
        public DateTimeViewModel ArticleDate { get; set; } = new DateTimeViewModel();
        public bool ShowDate { get; set; } = true;

        [Required, AllowHtml]
        public string Content { get; set; }

        public bool GoBack { get; set; }

        public NewsArticle ToArticle(NewsArticle originalArticle = null)
        {
            var newArticle = originalArticle ?? new NewsArticle();

            newArticle.Title = Title;
            newArticle.Content = Content;
            newArticle.ArticleDate = DateTime.SpecifyKind(ArticleDate.ToDateTime().GetValueOrDefault(), DateTimeKind.Local);
            newArticle.ShowDate = ShowDate;

            return newArticle;
        }

        public NewsArticleViewModel Set(NewsArticle article)
        {
            Id = article.RowKey;
            Title = article.Title;
            Content = article.Content;
            ArticleDate = new DateTimeViewModel(article.ArticleDate, article.ArticleDate);
            ShowDate = article.ShowDate;

            return this;
        }
    }
}
