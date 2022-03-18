using System.Collections.Generic;
using System.Linq;
using Edubase.Data.Entity;

namespace Edubase.Web.UI.Models.News
{
    public class NewsArticlesViewModel
    {
        public IEnumerable<NewsArticle> Articles { get; }
        public int LookupYear { get; set; }
        public bool IsPreview { get; set; }
        public NewsArticlesViewModel(IEnumerable<NewsArticle> items, int lookupYear, bool isPreview = false)
        {
            Articles = items.OrderByDescending(x => x.ArticleDate);
            LookupYear = lookupYear;
            IsPreview = isPreview;
        }
    }
}
