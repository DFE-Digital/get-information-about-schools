using System.Web;
using System.Web.Mvc;

namespace Edubase.Web.UI.Helpers
{
    public static class NonceHelper
    {
        public static IHtmlString ScriptNonce(this HtmlHelper helper)
        {
            var owinContext = helper.ViewContext.HttpContext.GetOwinContext();
            return new HtmlString(owinContext.Get<string>("ScriptNonce"));
        }
    }
}
