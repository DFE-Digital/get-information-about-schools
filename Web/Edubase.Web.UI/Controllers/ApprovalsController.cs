using Edubase.Web.UI.Filters;
using System.Web.Mvc;
using Edubase.Web.UI.Helpers;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("Approvals"), Route("{action=index}"), MvcAuthorizeRoles(AuthorizedRoles.CanApprove)]
    public class ApprovalsController : Controller
    {
        [HttpGet, EdubaseAuthorize, Route(Name = "PendingApprovals")]
        public ActionResult Index() => View();
    }
}
