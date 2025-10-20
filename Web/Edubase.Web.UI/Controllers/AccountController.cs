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
    [ApiController]
    [Route("account")]
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

        [HttpGet("login")]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = "/")
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl = returnUrl.Clean() });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet("external-login-callback")]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = "/")
        {
            var result = await HttpContext.AuthenticateAsync(OpenIdConnectDefaults.AuthenticationScheme);
            if (!result.Succeeded || result.Principal == null)
            {
                throw new EdubaseException("No external login information was obtainable.");
            }

            var identity = result.Principal.Identity as ClaimsIdentity;
            if (identity == null)
            {
                throw new EdubaseException("Invalid identity.");
            }

            var convertedIdentity = ConfigurationManager.AppSettings["owin:appStartup"] == "SASimulatorConfiguration"
                ? new StubClaimsIdConverter().Convert(identity)
                : new SecureAccessClaimsIdConverter().Convert(identity);

            var principal = new ClaimsPrincipal(convertedIdentity);
            var roles = await _securityService.GetRolesAsync(principal);
            convertedIdentity.AddClaims(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return await GetLandingPage(principal);
        }

        private async Task<IActionResult> GetLandingPage(ClaimsPrincipal principal)
        {
            var searchToken = (await _userPreferenceRepository.GetAsync(principal.GetUserId()))?.SavedSearchToken;
            if (searchToken != null && (await _tokenRepository.GetAsync(searchToken)) != null)
            {
                TempData["SavedToken"] = searchToken;
                TempData["UserId"] = principal.GetUserId();
                return Redirect($"{Url.RouteUrl("EstabSearch")}?tok={searchToken}");
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

        [HttpGet("logoff")]
        public async Task<IActionResult> LogOff()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
            return Redirect("/");
        }
    }
}
