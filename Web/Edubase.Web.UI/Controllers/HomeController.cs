using Edubase.Common.IO;
using Edubase.Data.Identity;
using Edubase.Services;
using Edubase.Services.Lucene;
using Edubase.Web.UI.Helpers;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
    public class HomeController : EduBaseController
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
            if (pwd == "c7634") return Json(MessageLoggingService.Instance.GetPending());
            else return new EmptyResult();
        }

        


        [HttpGet]
        public async Task<ActionResult> RebuildIndex()
        {
            var log = await EstablishmentsIndex.Instance.RebuildEstablishmentsIndexAsync();
            return Content(log.ToString(), "text/plain");
        }

        [HttpGet]
        public async Task<ActionResult> ReinitIndex()
        {
            var log = await EstablishmentsIndex.Instance.InitialiseAsync();
            return Content(log.ToString(), "text/plain");
        }



        public async Task FlushErrors() => await MessageLoggingService.Instance.FlushAsync();

    }
}