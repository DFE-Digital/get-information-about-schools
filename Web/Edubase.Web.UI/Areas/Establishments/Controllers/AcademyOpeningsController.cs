using Edubase.Services.Establishments;
using Edubase.Web.UI.Helpers;
using System.Web.Mvc;

namespace Edubase.Web.UI.Areas.Establishments.Controllers
{
    [RouteArea("Establishments"), RoutePrefix("manage"), Route("{action=index}"), MvcAuthorizeRoles(AuthorizedRoles.CanManageAcademyOpenings)]
    public class AcademyOpeningsController : Controller
    {
        readonly IEstablishmentWriteService _establishmentWriteService;

        public AcademyOpeningsController(IEstablishmentWriteService establishmentWriteService)
        {
            _establishmentWriteService = establishmentWriteService;
        }

        [HttpGet, Route("academy-openings", Name = "ManageAcademyOpenings")]
        public ActionResult Index() => View();
    }
}
