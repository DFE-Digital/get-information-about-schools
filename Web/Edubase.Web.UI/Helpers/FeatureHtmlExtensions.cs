using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Edubase.Common.Config;

namespace Edubase.Web.UI.Helpers
{
    public static class FeatureHtmlExtensions
    {
        public static bool FeatureEnabled(this HtmlHelper html, string feature)
            => Feature.IsEnabled(feature); // uses the static helper
    }
}
