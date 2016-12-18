using Edubase.Common.Cache;
using Edubase.Data.Repositories.Establishments;
using Edubase.Services;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
    public class HomeController : EduBaseController
    {
        public HomeController()
        {

        }

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