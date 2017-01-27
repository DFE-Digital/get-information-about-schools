using Edubase.Common.Cache;
using Edubase.Data.Repositories.Establishments;
using Edubase.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("Home"), Route("{action=index}")]
    public class HomeController : EduBaseController
    {
        public HomeController()
        {

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
    }
}