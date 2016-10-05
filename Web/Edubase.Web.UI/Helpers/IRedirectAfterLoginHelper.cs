using System.Web.Mvc;

namespace Edubase.Web.UI.Helpers
{
    public interface IRedirectAfterLoginHelper
    {
        ActionResult GetResult(string returnUrl);
    }
}