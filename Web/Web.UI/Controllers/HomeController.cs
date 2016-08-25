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
    }
}