using Edubase.Services.Security;
using Edubase.Web.UI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

namespace Edubase.Web.UI.Controllers.Api
{ 
    public class UserRolesController : EduBaseController
    {
        [HttpGet("api/should-show-data-status")]
        public IActionResult Index() => Json(new UserRolesViewModel(UserRequiresDataQualityPrompt(User)));

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
