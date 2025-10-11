using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("TermsofUse"), Route("{action=index}")]
    public class TermsofUseController : EduBaseController
    {
        [Route(Name = "TermsofUse")]
        public ActionResult Index() => View();
    }
}
