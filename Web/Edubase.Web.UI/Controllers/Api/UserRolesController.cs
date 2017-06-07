using Edubase.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Controllers.Api
{
  
    public class UserRolesController : Controller
    {
        
        [Route("api/should-show-data-status"), HttpGet]
        public JsonResult Index()
        {
            var model = new UserRolesViewModel
            {
                UserRequiresDataPrompt = User.InRole(
                        EdubaseRoles.EFADO,     // EFA data Owners
                        EdubaseRoles.AP_AOS,    // Academy openers
                        EdubaseRoles.IEBT,      // Independent schools
                        EdubaseRoles.APT,       // Alt provision team
                        EdubaseRoles.SOU,       // SOPT
                        EdubaseRoles.FST        // Free school openers
                    )
            };

            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}
