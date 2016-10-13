using Edubase.Data.Identity;
using Edubase.Services;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var model = new Models.HomepageViewModel();
            model.AllowApprovals = User.Identity.IsAuthenticated;
            model.AllowSchoolCreation = User.IsInRole(Roles.Admin) || User.IsInRole(Roles.LA);
            model.AllowTrustCreation = User.IsInRole(Roles.Admin) || User.IsInRole(Roles.Academy);
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

        public ActionResult DoException() { throw new System.Exception("This is a test exception"); }

        [HttpGet]
        public ActionResult GetPendingErrors(string pwd)
        {
            if (pwd == "c7634") return Json(MessageLoggingService.Instance.GetPending(), JsonRequestBehavior.AllowGet);
            else return new EmptyResult();
        }

        public async Task FlushErrors() => await MessageLoggingService.Instance.FlushAsync();
    }
}