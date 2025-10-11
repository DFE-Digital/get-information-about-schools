using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("Unauthorized"), Route("{action=index}")]
    public class UnauthorizedController : Controller
    {
        [HttpGet, Route("Index")]
        // GET: Unauthorized
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet, Route("LoginFailed")]
        public ActionResult LoginFailed()
        {
            return View();
        }
    }
}