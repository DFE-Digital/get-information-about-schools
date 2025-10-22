using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [Route("Unauthorized")]
    public class UnauthorizedController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("LoginFailed")]
        public IActionResult LoginFailed()
        {
            return View();
        }
    }
}
