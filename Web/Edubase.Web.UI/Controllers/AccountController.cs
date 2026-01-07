using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Edubase.Data.Repositories;
using Edubase.Services.Exceptions;
using Edubase.Services.Security;
using Edubase.Services.Texuna;
using Edubase.Web.UI.Authentication.ClaimsIdentityConverters;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Sustainsys.Saml2.AspNetCore2; // for Saml2Defaults

namespace Edubase.Web.UI.Controllers;

/// <summary>
/// Provides account-related actions such as login, external authentication callbacks,
/// landing page redirection, and logoff functionality.
/// </summary>
[ApiController]
[Route("Account")]
public class AccountController : Controller
{
    private readonly ISecurityService _securityService;
    private readonly IUserPreferenceRepository _userPreferenceRepository;
    private readonly ITokenRepository _tokenRepository;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountController"/> class.
    /// </summary>
    /// <param name="securityService">Service for handling security and role management.</param>
    /// <param name="userPreferenceRepository">Repository for accessing user preferences.</param>
    /// <param name="tokenRepository">Repository for managing saved search tokens.</param>
    public AccountController(
        ISecurityService securityService,
        IUserPreferenceRepository userPreferenceRepository,
        ITokenRepository tokenRepository,
        IConfiguration configuration)
    {
        _securityService = securityService;
        _userPreferenceRepository = userPreferenceRepository;
        _tokenRepository = tokenRepository;
        _configuration = configuration;
    }

    /// <summary>
    /// Initiates the login process using the SAML2 authentication scheme.
    /// </summary>
    /// <param name="returnUrl">The URL to redirect to after successful login.</param>
    /// <returns>An <see cref="IActionResult"/> that challenges the SAML2 scheme.</returns>
    [HttpGet("Login")]
    [AllowAnonymous]
    public IActionResult Login(string returnUrl)
    {
        string redirectUrl =
            Url.Action("ExternalLoginCallback", "Account", new { returnUrl });

        return Challenge(
            new AuthenticationProperties { RedirectUri = redirectUrl },
            Saml2Defaults.Scheme // "Saml2"
        );
    }

    /// <summary>
    /// Handles the callback from the external SAML2 login provider.
    /// Authenticates the user, assigns roles, and signs them into the application.
    /// </summary>
    /// <param name="returnUrl">The URL to redirect to after successful authentication.</param>
    /// <returns>An <see cref="IActionResult"/> that redirects the user to their landing page.</returns>
    [HttpGet("ExternalLoginCallback")]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalLoginCallback()
    {
        // Authenticate against the application cookie scheme
        AuthenticateResult loginInfo =
            await HttpContext.AuthenticateAsync("ApplicationCookie");

        if (loginInfo?.Principal == null)
        {
            throw new EdubaseException("No external login information was obtainable.");
        }

        ClaimsIdentity? identity =
            loginInfo.Principal.Identity as ClaimsIdentity ??
                throw new EdubaseException("No valid claims identity was obtainable.");

        // Use the new UseSimulatorAuth setting to choose the converter
        bool useSimulatorAuth = _configuration.GetValue<bool>("AppSettings:UseSimulatorAuth");

        ClaimsIdentity convertedIdentity = useSimulatorAuth
            ? new StubClaimsIdentityConverter().Convert(identity)
            : new SecureAccessClaimsIdConverter().Convert(identity);

        ClaimsPrincipal principal = new ClaimsPrincipal(convertedIdentity);

        // Add roles from the security service
        string[] roles = await _securityService.GetRolesAsync(principal);
        convertedIdentity.AddClaims(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // Re‑sign in with the application cookie scheme (refresh claims)
        await HttpContext.SignInAsync("ApplicationCookie", principal);

        return await GetLandingPage(principal);
    }

    /// <summary>
    /// Determines the appropriate landing page for the authenticated user
    /// based on their roles and saved preferences.
    /// </summary>
    /// <param name="principal">The authenticated user principal.</param>
    /// <returns>An <see cref="IActionResult"/> redirecting to the appropriate landing page.</returns>
    private async Task<IActionResult> GetLandingPage(ClaimsPrincipal principal)
    {
        string? searchToken =
            (await _userPreferenceRepository.GetAsync(
                principal.GetUserId()))?.SavedSearchToken;

        if (searchToken != null && (await _tokenRepository.GetAsync(searchToken)) != null)
        {
            TempData["SavedToken"] = searchToken;
            TempData["UserId"] = principal.GetUserId();
            return Redirect($"{Url.RouteUrl("EstabSearch")}?tok={searchToken}");
        }

        if (principal.IsInRole(EdubaseRoles.ESTABLISHMENT))
        {
            int? urn = await
                _securityService.GetMyEstablishmentUrn(principal);

            if (urn.HasValue)
            {
                return RedirectToRoute("EstabDetails", new { id = urn });
            }
        }
        else if (
            principal.IsInRole(EdubaseRoles.EDUBASE_GROUP_MAT) ||
            principal.IsInRole(EdubaseRoles.SSAT))
        {
            int? uid =
                await _securityService.GetMyMATUId(principal);

            if (uid.HasValue)
            {
                return RedirectToRoute("GroupDetails", new { id = uid });
            }
        }

        return RedirectToAction("Index", "Home");
    }

    /// <summary>
    /// Logs the user out of the application by clearing their authentication cookie.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> that redirects to the home page.</returns>
    [HttpGet("LogOff")]
    public async Task<IActionResult> LogOff()
    {
        await HttpContext.SignOutAsync("ApplicationCookie");
        return Redirect("/");
    }
}
