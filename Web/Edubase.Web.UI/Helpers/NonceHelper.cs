using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Edubase.Web.UI.Helpers
{
    public static class NonceHelper
    {
        public static HtmlString ScriptNonce(this IHtmlHelper helper)
        {
            var httpContext = helper.ViewContext.HttpContext;
            var nonce = httpContext.Items["ScriptNonce"] as string;

            return new HtmlString(nonce ?? string.Empty);
        }
    }
}
