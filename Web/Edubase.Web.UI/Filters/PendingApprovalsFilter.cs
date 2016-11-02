using Edubase.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Edubase.Web.UI.Filters
{
    public class PendingApprovalsFilter : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if(filterContext.HttpContext.User.Identity.IsAuthenticated)
                filterContext.Controller.ViewBag.PendingApprovalItemsCount = new ApprovalService().Count(filterContext.HttpContext.User as ClaimsPrincipal);
            base.OnResultExecuting(filterContext);
        }
    }
}