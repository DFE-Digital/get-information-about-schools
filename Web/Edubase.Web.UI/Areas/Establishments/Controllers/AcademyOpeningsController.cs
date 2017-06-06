using Edubase.Services.Establishments;
using Edubase.Services.Security;
using Edubase.Web.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Edubase.Web.UI.Areas.Establishments.Controllers
{
    [RouteArea("Establishments"), RoutePrefix("manage"), Route("{action=index}")]
    public class AcademyOpeningsController : Controller
    {
        readonly IEstablishmentWriteService _establishmentWriteService;

        public AcademyOpeningsController(IEstablishmentWriteService establishmentWriteService)
        {
            _establishmentWriteService = establishmentWriteService;
        }

        [HttpGet, Route("academy-openings", Name = "ManageAcademyOpenings"), MvcAuthorizeRoles(EdubaseRoles.AP_AOS, EdubaseRoles.ROLE_BACKOFFICE, EdubaseRoles.EFADO)]
        public ActionResult Index() => View();



    }
}