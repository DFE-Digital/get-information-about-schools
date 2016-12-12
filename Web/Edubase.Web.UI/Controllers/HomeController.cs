using Edubase.Services;
using Edubase.Services.Cache;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
    public class HomeController : EduBaseController
    {
        private CacheAccessor _cacheAccessor;

        public HomeController(CacheAccessor cacheAccessor)
        {
            _cacheAccessor = cacheAccessor;
        }

        public ActionResult Index()
        {
            var model = new Models.HomepageViewModel();
            model.AllowApprovals = User.Identity.IsAuthenticated;
            model.AllowSchoolCreation = User.Identity.IsAuthenticated;
            model.AllowTrustCreation = User.Identity.IsAuthenticated;
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

       
        [Authorize]
        public ActionResult Secure()
        {
            var identity = System.Web.HttpContext.Current.User.Identity as ClaimsIdentity;
            return View(identity.Claims);
        }

        [HttpGet]
        public ActionResult RedisStatus()
        {
            return Content($"Redis status: {_cacheAccessor.Status}");
        }

        public async Task FlushErrors() => await MessageLoggingService.Instance.FlushAsync();

    }
}