using System.Collections.Generic;
using System.Configuration;
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
        public string FinancialBenchmarkingHomepage => ConfigurationManager.AppSettings["FinancialBenchmarkingURL"];
        public string FscpHomepage => ConfigurationManager.AppSettings["FscpHomeUrl"];
    }
}
