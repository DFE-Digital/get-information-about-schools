using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Edubase.Web.UI.Models.News
{
    public class NewsArticleViewModel
    {
        public string Id { get; set; }
        [Required, AllowHtml]
        public string Title { get; set; }
        public DateTimeViewModel ArticleDate { get; set; }
        public bool ShowDate { get; set; } = true;

        [Required, AllowHtml]
        public string Content { get; set; }
    }
}
