using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [ApiController]
    [Route("unauthorized")]
    public class UnauthorizedController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("loginfailed")]
        public IActionResult LoginFailed()
        {
            return View();
        }
    }
}
