using Edubase.Data.Identity;
using Edubase.Services.Security;
using Edubase.Services.Security.ClaimsIdentityConverters;
using Edubase.Web.UI.Helpers;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Edubase.Web.UI.Controllers
{
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
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            return new ChallengeResult(AuthenticationManager.GetExternalAuthenticationTypes()
                .First().AuthenticationType, 
                Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = "/Search" }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            var id = loginInfo.ExternalIdentity;

            // todo: when SA is enabled, convert to our json based claim tokens
            id = await new SecurityService().LoginAsync(id, new StubClaimsIdConverter(), UserManager); // todo: SecureAccessClaimsIdConverter
            
            AuthenticationManager.SignIn(id);

            var urlHelper = new UrlHelper(Request.RequestContext);
            if (urlHelper.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else return RedirectToAction("Index", "Search");
        }
        
        
        [HttpGet]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(new AuthenticationProperties { RedirectUri = "/Search" });
            return RedirectToAction("Index", "Search");
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

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;
        
        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}
