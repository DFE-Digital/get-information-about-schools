using Edubase.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Edubase.Web.UI.Models;
using System.Security.Principal;

namespace Edubase.Web.UI.Controllers.Api
{ 
    public class UserRolesController : EduBaseController
    {   
        [Route("api/should-show-data-status"), HttpGet]
        public ActionResult Index() => Json(new UserRolesViewModel(UserRequiresDataQualityPrompt(User)));
        
        internal static bool UserRequiresDataQualityPrompt(IPrincipal principal) => principal.InRole(
                        EdubaseRoles.EFADO,     // EFA data Owners
                        EdubaseRoles.AP_AOS,    // Academy openers
                        EdubaseRoles.IEBT,      // Independent schools
                        EdubaseRoles.APT,       // Alt provision team
                        EdubaseRoles.SOU,       // SOPT
                        EdubaseRoles.FST,       // Free school openers
                        EdubaseRoles.YCS        // Youth custody service
                    );
    }
}
