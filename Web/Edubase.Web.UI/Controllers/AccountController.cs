using Edubase.Common;
using Edubase.Services.Security;
using Edubase.Services.Security.ClaimsIdentityConverters;
using Edubase.Web.UI.MvcResult;
using Microsoft.Owin.Security;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System;
using Edubase.Services.Enums;

namespace Edubase.Web.UI.Controllers
{
    [RoutePrefix("Account")]
    public class AccountController : Controller
    {
        private readonly ISecurityService _securityService;

        public AccountController(ISecurityService securityService)
        {
            _securityService = securityService;
        }

        //
        // GET: /Account/Login
        [Route(nameof(Login)), AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            return new ChallengeResult(AuthenticationManager.GetExternalAuthenticationTypes()
                .First().AuthenticationType, 
                Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl.Clean() ?? "/Search" }));
        }

        [Route(nameof(ExternalLoginCallback)), AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            var id = loginInfo.ExternalIdentity;

            if (ConfigurationManager.AppSettings["owin:appStartup"] == "SASimulatorConfiguration") id = new StubClaimsIdConverter().Convert(id);
            else id = new SecureAccessClaimsIdConverter().Convert(id);

            var principal = new ClaimsPrincipal(id);
            var roles = await _securityService.GetRolesAsync(principal);
            id.AddClaims(roles.Select(x => new Claim(ClaimTypes.Role, x)));
            
            AuthenticationManager.SignIn(id);

            return GetLandingPage(principal);
        }

        private ActionResult GetLandingPage(ClaimsPrincipal principal)
        {
            if (principal.IsInRole(EdubaseRoles.ESTABLISHMENT))
            {
                var urn = _securityService.GetMyEstablishmentUrn(principal);
                return RedirectToRoute("EstabDetails", new { id = urn });
            }
            else if (principal.IsInRole(EdubaseRoles.EDUBASE_GROUP_MAT))
            {
                var uid = _securityService.GetMyMATUId(principal);
                return RedirectToRoute("GroupDetails", new { id = uid });
            }
            else if (principal.IsInRole(EdubaseRoles.IEBT))
            {
                var types = new[]{eLookupEstablishmentType.NonmaintainedSpecialSchool

                return Redirect("/Establishments/Search?a=31&a=5&a=10&a=7");
            }
            else return RedirectToAction("Index", "Search");
        }

        [Route(nameof(LogOff)), HttpGet]
        public ActionResult LogOff(string returnUrl)
        {
            AuthenticationManager.SignOut(new AuthenticationProperties { RedirectUri = "/" });
            return Redirect("/");
        }
        
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;
        
    }
}
