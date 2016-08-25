using System.Web.Mvc;

namespace Web.UI.Helpers
{
    public interface IRedirectAfterLoginHelper
    {
        ActionResult GetResult(string returnUrl);
    }
}