using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Edubase.Data.Repositories;
using Edubase.Services.Exceptions;
using Edubase.Services.Security;
using Edubase.Services.Texuna;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Controllers
{
    [ApiController]
    [Route("Account")]
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

        [HttpGet("Login")]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            // Replace with ASP.NET Core external login challenge
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { returnUrl });
            var provider = "SecureAccess"; // Replace with actual provider name
            return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, provider);
        }

        [HttpGet("ExternalLoginCallback")]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await HttpContext.AuthenticateAsync(); // Replace with actual external login retrieval
            if (loginInfo?.Principal == null)
            {
                throw new EdubaseException("No external login information was obtainable.");
            }

            var identity = loginInfo.Principal.Identity as ClaimsIdentity;
            var principal = new ClaimsPrincipal(identity);

            var roles = await _securityService.GetRolesAsync(principal);
            identity.AddClaims(roles.Select(x => new Claim(ClaimTypes.Role, x)));

            await HttpContext.SignInAsync(principal);

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

        [HttpGet("LogOff")]
        public async Task<IActionResult> LogOff()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}
