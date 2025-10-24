using System.Linq;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Edubase.Web.UI.Views.TagHelpers;

[HtmlTargetElement("css-spinner")]
public class CssSpinnerTagHelper : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "div";
        output.Attributes.SetAttribute("id", "loading-spinner");
        output.Attributes.SetAttribute("class", "lds-spinner");

        string spinnerHtml = string.Join("", Enumerable.Repeat("<div></div>", 12));
        output.Content.SetHtmlContent(spinnerHtml);
    }
}
