using Edubase.Services;
using StackExchange.Profiling;
using System.Security.Claims;
using System.Web.Mvc;

namespace Edubase.Web.UI.Filters
{
    public class PendingApprovalsFilter : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                using (MiniProfiler.Current.Step("Getting PendingApprovalItemsCount in PendingApprovalsFilter"))
                {
                    filterContext.Controller.ViewBag.PendingApprovalItemsCount = new ApprovalService().Count(filterContext.HttpContext.User as ClaimsPrincipal);
                }
            }
            base.OnResultExecuting(filterContext);
        }
    }
}