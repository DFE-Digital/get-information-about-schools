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
using Edubase.Web.UI.Areas.Establishments.Models.Search;

namespace Edubase.Web.UI.Controllers
{
    using ET = eLookupEstablishmentType;
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

            return await GetLandingPage(principal);
        }

        private async Task<ActionResult> GetLandingPage(ClaimsPrincipal principal)
        {
            if (principal.IsInRole(EdubaseRoles.ESTABLISHMENT))
            {
                var urn = await _securityService.GetMyEstablishmentUrn(principal);
                if (urn.HasValue) return RedirectToRoute("EstabDetails", new { id = urn });
            }
            else if (principal.IsInRole(EdubaseRoles.EDUBASE_GROUP_MAT))
            {
                var uid = await _securityService.GetMyMATUId(principal);
                if (uid.HasValue) return RedirectToRoute("GroupDetails", new { id = uid });
            }
            else if (principal.IsInRole(EdubaseRoles.IEBT))
            {
                var searchQueryString = string.Join("&", new[] { ET.NonmaintainedSpecialSchool, ET.BritishSchoolsOverseas, ET.CityTechnologyCollege, ET.OtherIndependentSchool }
                    .Select(x => $"{EstablishmentSearchViewModel.BIND_ALIAS_TYPEIDS}={(int)x}"));
                return Redirect(string.Concat(Url.RouteUrl("EstabSearch"), "?", searchQueryString));
            }

            return RedirectToAction("Index", "Search");
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
