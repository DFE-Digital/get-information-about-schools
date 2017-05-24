using Edubase.Common;
using Edubase.Services.Security;
using Edubase.Services.Security.ClaimsIdentityConverters;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.MvcResult;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

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

            var roles = await _securityService.GetRolesAsync(new ClaimsPrincipal(id));
            id.AddClaims(roles.Select(x => new Claim(ClaimTypes.Role, x)));
            
            AuthenticationManager.SignIn(id);

            var urlHelper = new UrlHelper(Request.RequestContext);
            if (urlHelper.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            else return RedirectToAction("Index", "Search");
        }

        [Route(nameof(LogOff)), HttpGet]
        public ActionResult LogOff(string returnUrl)
        {
            AuthenticationManager.SignOut(new AuthenticationProperties { RedirectUri = returnUrl.Clean() ?? "/Search" });
            return Url.IsLocalUrl(returnUrl) ? (ActionResult) Redirect(returnUrl) : RedirectToAction("Index", "Search");
        }
        
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;
        
    }
}
