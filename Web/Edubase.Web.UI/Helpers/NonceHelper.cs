using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Edubase.Web.UI.Helpers
{
    public static class NonceHelper
    {
        public static IHtmlContent ScriptNonce(this IHtmlHelper helper)
        {
            var nonce = helper.ViewContext.HttpContext.Items["ScriptNonce"] as string;
            return string.IsNullOrEmpty(nonce) ? HtmlString.Empty : new HtmlString(nonce);
        }
    }
}
