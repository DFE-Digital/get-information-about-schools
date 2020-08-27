using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("Guidance"), Route("{action=index}")]
    public class GuidanceController : EduBaseController
    {
        [Route(Name = "Guidance")]
        public ActionResult Index() => View();
       
        public ActionResult General() => View();

        public ActionResult EstablishmentBulkUpdate() => View();

        public ActionResult ChildrensCentre() => View();
        public ActionResult Federation() => View();
        public ActionResult Governance() => View();
    }
}
