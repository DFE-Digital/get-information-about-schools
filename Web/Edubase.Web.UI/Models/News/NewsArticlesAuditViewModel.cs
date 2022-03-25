using System.Collections.Generic;
using System.Linq;
using Edubase.Data.Entity;
using Edubase.Web.UI.Helpers;

namespace Edubase.Web.UI.Models.News
{
    public class NewsArticlesAuditViewModel
    {
        public List<NewsArticle> GroupedArticles { get; }

        public NewsArticlesAuditViewModel(IEnumerable<NewsArticle> items, string sortBy)
        {
            var sortOrder = "desc";
            if (!string.IsNullOrEmpty(sortBy))
            {
                var split = string.Concat(sortBy, "-").Split('-');
                sortBy = split[0];
                sortOrder = split[1];
            }

            if (sortOrder == "desc")
            {
                switch (sortBy)
                {
                    case nameof(NewsArticle.Tracker):
                        items = items.OrderByDescending(x => x.Tracker);
                        break;

                    case nameof(NewsArticle.AuditTimestamp):
                        items = items.OrderByDescending(x => x.AuditTimestamp);
                        break;

                    case nameof(NewsArticle.Status):
                        items = items.OrderByDescending(x => x.Status.EnumDisplayNameFor());
                        break;

                    case nameof(NewsArticle.AuditUser):
                        items = items.OrderByDescending(x => x.AuditUser);
                        break;

                    case nameof(NewsArticle.Title):
                        items = items.OrderByDescending(x => x.Title);
                        break;

                    case nameof(NewsArticle.ArticleDate):
                        items = items.OrderByDescending(x => x.ArticleDate);
                        break;

                    case nameof(NewsArticle.ShowDate):
                        items = items.OrderByDescending(x => x.ShowDate);
                        break;

                    case nameof(NewsArticle.Content):
                        items = items.OrderByDescending(x => x.Content);
                        break;

                    default:
                        items = items.OrderByDescending(x => x.ArticleDate);
                        break;
                }
            }
            else
            {
                switch (sortBy)
                {
                    case nameof(NewsArticle.Tracker):
                        items = items.OrderBy(x => x.Tracker);
                        break;

                    case nameof(NewsArticle.AuditTimestamp):
                        items = items.OrderBy(x => x.AuditTimestamp);
                        break;

                    case nameof(NewsArticle.Status):
                        items = items.OrderBy(x => x.Status.EnumDisplayNameFor());
                        break;

                    case nameof(NewsArticle.AuditUser):
                        items = items.OrderBy(x => x.AuditUser);
                        break;

                    case nameof(NewsArticle.Title):
                        items = items.OrderBy(x => x.Title);
                        break;

                    case nameof(NewsArticle.ArticleDate):
                        items = items.OrderBy(x => x.ArticleDate);
                        break;

                    case nameof(NewsArticle.ShowDate):
                        items = items.OrderBy(x => x.ShowDate);
                        break;

                    case nameof(NewsArticle.Content):
                        items = items.OrderBy(x => x.Content);
                        break;
                        
                    default:
                        items = items.OrderBy(x => x.ArticleDate);
                        break;
                }
            }

            GroupedArticles = items.ToList();
        }
    }
}
