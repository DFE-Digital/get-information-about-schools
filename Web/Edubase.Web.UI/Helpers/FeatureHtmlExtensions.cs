using Edubase.Common.Config;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Edubase.Web.UI.Helpers
{
    public static class FeatureHtmlExtensions
    {
        public static bool FeatureEnabled(this IHtmlHelper html, string feature)
            => Feature.IsEnabled(feature);
    }
}
