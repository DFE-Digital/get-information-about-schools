using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [Route("TermsofUse")]
    public class TermsofUseController : EduBaseController
    {
        [HttpGet("", Name = "TermsofUse")]
        public IActionResult Index() => View();
    }
}
