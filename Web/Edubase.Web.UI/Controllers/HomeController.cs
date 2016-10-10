using Edubase.Data.Identity;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var model = new Models.HomepageViewModel();
            model.AllowApprovals = User.Identity.IsAuthenticated;
            model.AllowCreation = User.IsInRole(Roles.Admin);
            return View(model);
        }

        [HttpGet]
        public ActionResult MATAdmin(int id)
        {
            ViewBag.Message = $"As a MAT Administrator (MAT ID:{id}) you'll soon be able to see a list of schools on this page";
            return View("Placeholder");
        }

        [HttpGet]
        public ActionResult LAAdmin(int id)
        {
            ViewBag.Message = $"As an LA Administrator for LA ID {id} you'll soon be able to see a list of schools on this page";
            return View("Placeholder");
        }
    }
}