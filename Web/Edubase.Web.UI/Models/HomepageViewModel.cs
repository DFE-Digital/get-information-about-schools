using System.Collections.Generic;
using Edubase.Data.Entity;

namespace Edubase.Web.UI.Models
{
    public class HomepageViewModel
    {
        public HomepageViewModel(IEnumerable<NewsArticle> newsArticles)
        {
            NewsArticles = newsArticles;
        }
        public IEnumerable<NewsArticle> NewsArticles { get; set; }
        public string FinancialBenchmarkingHomepage { get; set; }
        public string FscpdServiceName { get; set; }
        public string FscpdHomepage { get; set; }
    }
}
