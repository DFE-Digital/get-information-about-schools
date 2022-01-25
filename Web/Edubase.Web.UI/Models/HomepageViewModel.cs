using System.Configuration;

namespace Edubase.Web.UI.Models
{
    public class HomepageViewModel
    {
        public string FinancialBenchmarkingHomepage => ConfigurationManager.AppSettings["FinancialBenchmarkingURL"];

        public string CscpHomepage => ConfigurationManager.AppSettings["CscpHomeUrl"];
    }
}
