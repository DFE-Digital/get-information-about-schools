using System.Web.Mvc;

namespace Web.UI.Controllers
{
    public class UnauthorizedController : Controller
    {
        // GET: Unauthorized
        public ActionResult Index()
        {
            return View();
        }
    }
}