using System.Web.Mvc;
using System.Web.Routing;
using Web.Identity;
using Web.UI.Utils;

namespace Web.UI.Helpers
{
    public class RedirectAfterLoginHelper : IRedirectAfterLoginHelper
    {
        private readonly IUserIdentity _userIdentity;
        private readonly IRequestContext _requestContext;

        public RedirectAfterLoginHelper(IUserIdentity userIdentity, IRequestContext requestContext)
        {
            _userIdentity = userIdentity;
            _requestContext = requestContext;
        }

        public ActionResult GetResult(string returnUrl)
        {
            var urlHelper = new UrlHelper(_requestContext.GetContext());
            if (!string.IsNullOrEmpty(returnUrl) && urlHelper.IsLocalUrl(returnUrl))
            {
                return new RedirectResult(returnUrl);
            }

            var claim = _userIdentity.FindFirstClaim(IdentityConstants.AccessibleSchoolIdsClaimTypeName);
            if(claim != null && !string.IsNullOrWhiteSpace(claim.Value))
            {
                var schoolIds = claim.Value.Split(',');
                var schoolId = schoolIds[0];
                return RedirectTo("Schools", "Details", schoolId);
            }

            claim = _userIdentity.FindFirstClaim(IdentityConstants.ClaimTypeNameMATAdmin);
            if (claim != null && !string.IsNullOrWhiteSpace(claim.Value))
            {
                var ids = claim.Value.Split(',');
                var id = ids[0];
                return RedirectTo("Home", "MATAdmin", id);
            }

            claim = _userIdentity.FindFirstClaim(IdentityConstants.ClaimTypeNameLAAdmin);
            if (claim != null && !string.IsNullOrWhiteSpace(claim.Value))
            {
                var ids = claim.Value.Split(',');
                var id = ids[0];
                return RedirectTo("Home", "LAAdmin", id);
            }

            if (_userIdentity.IsInRole(IdentityConstants.AccessAllSchoolsRoleName))
            {
                return RedirectTo("Search");
            }

            return RedirectTo("Home");
        }

        private static RedirectToRouteResult RedirectTo(string controller, string action = "Index", string id = "")
        {
            return new RedirectToRouteResult(null, new RouteValueDictionary
                {
                    {"action", action},
                    {"controller", controller},
                    {"id", id }
                });
        }
    }
}