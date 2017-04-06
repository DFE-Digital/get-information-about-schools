using Edubase.Common;
using Edubase.Data.Identity;
using Edubase.Services.Security;
using Edubase.Services.Security.ClaimsIdentityConverters;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UI.MvcResult;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
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
        
        public AccountController()
        {

        }

        public AccountController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
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

            // todo: when SA is enabled, convert to our json based claim tokens
            id = await new SecurityService().LoginAsync(id, new StubClaimsIdConverter(), UserManager); // todo: SecureAccessClaimsIdConverter
            
            AuthenticationManager.SignIn(id);

            var urlHelper = new UrlHelper(Request.RequestContext);
            if (urlHelper.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            else return RedirectToAction("Index", "Search");
        }
        
        /*
         *  NOTE: THIS IS A V. FAST LOGIN API FOR QA PURPOSES ONLY. THIS WILL BE REMOVED IN DUE COURSE.
         * 
         */
        private Lazy<StubUserBuilder.Config> _stubUserConfig = new Lazy<StubUserBuilder.Config>(() => new StubUserBuilder.Configurator().Configure());

        [Route(nameof(QA_Login)), AllowAnonymous]
        public async Task<ActionResult> QA_Login(string username)
        {
            var u = _stubUserConfig.Value.UserList.FirstOrDefault(x => x.Assertion.NameId == username);
            if (u == null) return Content($"The username '{username}' was not found; choose from: " + string.Join(", ", _stubUserConfig.Value.UserList.Select(x => x.Assertion.NameId)), "text/plain");
            
            var id = await new SecurityService().LoginAsync(u.ToClaimsIdentity(), new StubClaimsIdConverter(), UserManager);

            AuthenticationManager.SignIn(id);

            return Content($"You are now logged in as {username} and have the following claims: \r\n" 
                + string.Join(",\r\n", id.Claims.Select(x=> $"Type: {x.Type}, Value: {x.Value}")), "text/plain");
        }
        // --------------------------------------------------------------------------------------------------------------------------------


        [Route(nameof(LogOff)), HttpGet]
        public ActionResult LogOff(string returnUrl)
        {
            AuthenticationManager.SignOut(new AuthenticationProperties { RedirectUri = returnUrl.Clean() ?? "/Search" });
            return Url.IsLocalUrl(returnUrl) ? (ActionResult) Redirect(returnUrl) : RedirectToAction("Index", "Search");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }
            base.Dispose(disposing);
        }
        
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;
        
    }
}
