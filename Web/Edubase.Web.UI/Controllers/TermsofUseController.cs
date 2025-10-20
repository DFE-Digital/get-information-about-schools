using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [ApiController]
    [Route("termsofuse")]
    public class TermsofUseController : EduBaseController
    {
        [HttpGet("", Name = "TermsofUse")]
        public IActionResult Index() => View();
    }
}
