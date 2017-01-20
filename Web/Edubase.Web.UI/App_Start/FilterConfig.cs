using Edubase.Web.UI.Filters;
using System.Configuration;
using System.Web.Mvc;

namespace Edubase.Web.UI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new UnauthorizedAccessAttribute());
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new PendingApprovalsFilter());
            if(bool.Parse(ConfigurationManager.AppSettings["EnableErrorReporting"])) filters.Add(new ExceptionHandler());
        }
    }
}
