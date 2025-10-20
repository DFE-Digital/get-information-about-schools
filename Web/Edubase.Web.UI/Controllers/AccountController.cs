using Edubase.Common;
using Edubase.Services.Security;
using Edubase.Services.Security.ClaimsIdentityConverters;
using Edubase.Web.UI.MvcResult;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System;
using Edubase.Services.Enums;
using Edubase.Web.UI.Areas.Establishments.Models.Search;
using Edubase.Services.Exceptions;
using Edubase.Data.Repositories;
using Edubase.Services.Texuna;
using Edubase.Web.UI.Models.Search;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers
{
    using ET = eLookupEstablishmentType;
    using ES = eLookupEstablishmentStatus;
    [RoutePrefix("Account")]
    public class AccountController : Controller
    {
        private readonly ISecurityService _securityService;
        private readonly IUserPreferenceRepository _userPreferenceRepository;
        private readonly ITokenRepository _tokenRepository;

        public AccountController(ISecurityService securityService, IUserPreferenceRepository userPreferenceRepository, ITokenRepository tokenRepository)
        {
            _securityService = securityService;
            _userPreferenceRepository = userPreferenceRepository;
            _tokenRepository = tokenRepository;
        }

        //
        // GET: /Account/Login
        [Route(nameof(Login)), AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            return new ChallengeResult(AuthenticationManager.GetExternalAuthenticationTypes()
                .First().AuthenticationType,
                Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl.Clean() ?? "/" }));
        }

        [Route(nameof(ExternalLoginCallback)), AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                throw new EdubaseException("No external login information was obtainable.");
            }

            var id = loginInfo.ExternalIdentity;

            id = ConfigurationManager.AppSettings["owin:appStartup"] == "SASimulatorConfiguration"
                ? new StubClaimsIdConverter().Convert(id)
                : new SecureAccessClaimsIdConverter().Convert(id);

            var principal = new ClaimsPrincipal(id);
            var roles = await _securityService.GetRolesAsync(principal);
            id.AddClaims(roles.Select(x => new Claim(ClaimTypes.Role, x)));

            AuthenticationManager.SignIn(id);

            return await GetLandingPage(principal);
        }

        private async Task<ActionResult> GetLandingPage(ClaimsPrincipal principal)
        {
            // Redirect to the user's last saved search token if there is one.
            var searchToken = (await _userPreferenceRepository.GetAsync(principal.GetUserId()))?.SavedSearchToken;
            if (searchToken != null && (await _tokenRepository.GetAsync(searchToken)) != null)
            {
                TempData["SavedToken"] = searchToken;
                TempData["UserId"] = principal.GetUserId();
                return Redirect(string.Concat(Url.RouteUrl("EstabSearch"), "?tok=", searchToken));
            }

            if (principal.IsInRole(EdubaseRoles.ESTABLISHMENT))
            {
                var urn = await _securityService.GetMyEstablishmentUrn(principal);
                if (urn.HasValue)
                {
                    return RedirectToRoute("EstabDetails", new { id = urn });
                }
            }
            else if (principal.IsInRole(EdubaseRoles.EDUBASE_GROUP_MAT) || principal.IsInRole(EdubaseRoles.SSAT))
            {
                //The group of SSAT works in a similar way to EDUBASE_GROUP_MAT where the landing page is the establishment group assigned to the user
                //it was not possible to use EDUBASE_GROUP_MAT because the permissions\field visibility on fields etc are different
                var uid = await _securityService.GetMyMATUId(principal);
                if (uid.HasValue)
                {
                    return RedirectToRoute("GroupDetails", new { id = uid });
                }
            }

            return RedirectToAction("Index", "Home");
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
