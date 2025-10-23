using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Edubase.Web.UI.Helpers
{
    public static class ServiceNavigationItem
    {
        /// <summary>
        /// Builds a GOV.UK service navigation list item with active state logic.
        /// </summary>
        public static HtmlString NavigationItem(this IHtmlHelper helper, string currentSiteSection, string siteSection, string linkText,
            string action, string controller, string id)
        {
            var active = siteSection == currentSiteSection;

            var sb = new StringBuilder("<li class=\"govuk-service-navigation__item");

            if (active)
            {
                sb.Append(" govuk-service-navigation__item--active");
            }

            sb.Append("\">");

            var urlHelperFactory = (IUrlHelperFactory) helper.ViewContext.HttpContext.RequestServices.GetService(typeof(IUrlHelperFactory));
            var urlHelper = urlHelperFactory.GetUrlHelper(helper.ViewContext);
            var url = urlHelper.Action(action, controller, new { area = "" });

            sb.Append($"<a class=\"govuk-service-navigation__link\" href=\"{url}\" id=\"{id}\">");

            if (active)
            {
                sb.Append($"<strong class=\"govuk-service-navigation__active-fallback\">{linkText}</strong></a>");
            }
            else
            {
                sb.Append($"{linkText}</a>");
            }

            sb.Append("</li>");

            return new HtmlString(sb.ToString());
        }
    }
}
