using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
    public class UnauthorizedController : Controller
    {
        // GET: Unauthorized
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoginFailed()
        {
            return View();
        }
    }
}