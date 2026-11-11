using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [Route("Unauthorized")]
    public class UnauthorizedController : Controller
    {
        [HttpGet("")]
        [HttpGet("Index")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet("LoginFailed")]
        public ActionResult LoginFailed()
        {
            return View();
        }
    }
}
