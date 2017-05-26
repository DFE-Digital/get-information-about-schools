using Edubase.Web.UI.Filters;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("Approvals"), Route("{action=index}")]
    public class ApprovalsController : Controller
    {
        [HttpGet, EdubaseAuthorize, Route(Name = "PendingApprovals")]
        public ActionResult Index() => View();
    }
}