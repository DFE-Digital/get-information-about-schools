using System.Web.Mvc;

namespace Web.UI.Controllers
{
    public class HomeController : Controller
    {
        /*
        public StyleGuideController()
        {
            Get["/public/assets/styleguide"] = _ => Response.AsFile("public/assets/styleguide/base.html", "text/html");
        }
        */

        public ActionResult Index()
        {
            return View();
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