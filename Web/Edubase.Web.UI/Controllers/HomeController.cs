using Edubase.Services.Lookup;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("Home"), Route("{action=index}")]
    public class HomeController : EduBaseController
    {
        private readonly ILookupService _lookup;

        public HomeController(ILookupService lookup)
        {
            _lookup = lookup;
        }

        [Route]
        public ActionResult Index()
        {
            var model = new Models.HomepageViewModel();
            model.AllowApprovals = User.Identity.IsAuthenticated;
            model.AllowSchoolCreation = User.Identity.IsAuthenticated;
            model.AllowTrustCreation = User.Identity.IsAuthenticated;
            return View(model);
        }

        [Route("~/cookies")]
        public ActionResult Cookies()
        {
            return View();
        }

        [Route("~/guidance")]
        public ActionResult Guidance()
        {
            return View();
        }
        
    }
}