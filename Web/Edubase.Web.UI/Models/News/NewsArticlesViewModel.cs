using System.Collections.Generic;
using System.Linq;
using Edubase.Data.Entity;

namespace Edubase.Web.UI.Models.News
{
    public class NewsArticlesViewModel
    {
        public IEnumerable<NewsArticle> Articles { get; }

        public NewsArticlesViewModel(IEnumerable<NewsArticle> items)
        {
            Articles = items.OrderByDescending(x => x.ArticleDate);
        }
    }
}
