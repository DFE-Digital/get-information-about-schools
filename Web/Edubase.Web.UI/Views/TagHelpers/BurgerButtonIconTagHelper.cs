using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Edubase.Web.UI.Views.TagHelpers;

[HtmlTargetElement("burger-button-icon")]
public class BurgerButtonIconTagHelper : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "svg";
        output.Attributes.SetAttribute("width", "20px");
        output.Attributes.SetAttribute("height", "20px");
        output.Attributes.SetAttribute("viewBox", "0 0 20 20");
        output.Attributes.SetAttribute("xmlns", "http://www.w3.org/2000/svg");
        output.Attributes.SetAttribute("focusable", "false");
        output.Attributes.SetAttribute("role", "presentation");

        output.Content.SetHtmlContent(@"
            <g stroke=""none"" stroke-width=""1"" fill=""none"" fill-rule=""evenodd"">
                <g id=""Group"" fill=""currentColor"">
                    <polygon points=""0 0.636363636 20 0.636363636 20 5.63636364 0 5.63636364""></polygon>
                    <polygon points=""0 7.81818182 20 7.81818182 20 12.8181818 0 12.8181818""></polygon>
                    <polygon points=""0 15 20 15 20 20 0 20""></polygon>
                </g>
            </g>
        ");
    }
}

