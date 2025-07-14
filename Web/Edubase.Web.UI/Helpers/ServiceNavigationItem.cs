using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Edubase.Web.UI.Helpers
{
    public static class ServiceNavigationItem
    {
        /// <summary>
        /// After the GDS upgrade to 5 this logic was getting complex.
        /// Moved it here to allow testing and make the cshtml less complex.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="currentSiteSection"></param>
        /// <param name="siteSection"></param>
        /// <param name="linkText">text that appears on the link</param>
        /// <param name="action">mvc action</param>
        /// <param name="controller">mvc controller</param>
        /// <param name="id">id attribute of the a tag</param>
        /// <returns></returns>
        public static IHtmlString NavigationItem(this HtmlHelper helper, string currentSiteSection, string siteSection, string linkText,
            string action, string controller, string id)
        {
            var active = siteSection == currentSiteSection;

            var s = new StringBuilder("<li class=\"govuk-service-navigation__item");

            if(active)
            {
                s.Append(" govuk-service-navigation__item--active");
            }

            s.Append("\">");

            var uh = new UrlHelper(helper.ViewContext.RequestContext, helper.RouteCollection);
            var url = uh.Action(action, controller);

            s.Append($"<a class=\"govuk-service-navigation__link\" href=\"{url}\" id=\"{id}\">");

            if(active)
            {
                s.Append($"<strong class=\"govuk-service-navigation__active-fallback\">{linkText}</strong></a>");
            }
            else
            {
                s.Append($"{linkText}</a>");
            }

            s.Append("</li>");

            return new MvcHtmlString(s.ToString());
        }
    }
}
