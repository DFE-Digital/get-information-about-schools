using System;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Edubase.Common;
using Edubase.Data.Repositories;
using Edubase.Services.Exceptions;
using Edubase.Services.Security;
using Edubase.Services.Security.ClaimsIdentityConverters;
using Edubase.Services.Texuna;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [Route("Account")]
    public class AccountController : Controller
    {
        private readonly ISecurityService _securityService;
        private readonly IUserPreferenceRepository _userPreferenceRepository;
        private readonly ITokenRepository _tokenRepository;

        public AccountController(
            ISecurityService securityService,
            IUserPreferenceRepository userPreferenceRepository,
            ITokenRepository tokenRepository)
        {
            _securityService = securityService;
            _userPreferenceRepository = userPreferenceRepository;
            _tokenRepository = tokenRepository;
        }

        [HttpGet(nameof(Login))]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            var redirectUri = Url.Action(nameof(ExternalLoginCallback), "Account", new { ReturnUrl = returnUrl.Clean() ?? "/" });
            return new Edubase.Web.UI.MvcResult.ChallengeResult(GetExternalAuthenticationScheme(), redirectUri);
        }

        [HttpGet(nameof(ExternalLoginCallback))]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl)
        {
            var authResult = await AuthenticateExternalPrincipalAsync();
            var principal = authResult?.Principal;
            if (principal?.Identity is not ClaimsIdentity identity)
            {
                throw new EdubaseException("No external login information was obtainable.");
            }

            identity = ConfigurationManager.AppSettings["owin:appStartup"] == "SASimulatorConfiguration"
                ? new StubClaimsIdConverter().Convert(identity)
                : new SecureAccessClaimsIdConverter().Convert(identity);

            principal = new ClaimsPrincipal(identity);
            var roles = await _securityService.GetRolesAsync(principal);
            identity.AddClaims(roles.Select(x => new Claim(ClaimTypes.Role, x)));

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties());

            return await GetLandingPage(principal);
        }

        private async Task<IActionResult> GetLandingPage(ClaimsPrincipal principal)
        {
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
                var uid = await _securityService.GetMyMATUId(principal);
                if (uid.HasValue)
                {
                    return RedirectToRoute("GroupDetails", new { id = uid });
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet(nameof(LogOff))]
        public async Task<IActionResult> LogOff(string returnUrl)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = "/" });
            return Redirect("/");
        }

        private string GetExternalAuthenticationScheme()
        {
            var configuredScheme = ConfigurationManager.AppSettings["ExternalAuthenticationScheme"];
            return string.IsNullOrWhiteSpace(configuredScheme) ? null : configuredScheme;
        }

        private Task<AuthenticateResult> AuthenticateExternalPrincipalAsync()
        {
            var scheme = GetExternalAuthenticationScheme();
            return string.IsNullOrWhiteSpace(scheme)
                ? HttpContext.AuthenticateAsync()
                : HttpContext.AuthenticateAsync(scheme);
        }
    }
}
