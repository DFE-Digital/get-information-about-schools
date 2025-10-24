using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Edubase.Web.UI.Views.TagHelpers;

[HtmlTargetElement("filter-icon")]
public class FilterIconTagHelper : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "svg";
        output.Attributes.SetAttribute("width", "20");
        output.Attributes.SetAttribute("height", "20");
        output.Attributes.SetAttribute("viewBox", "0 0 30 30");
        output.Attributes.SetAttribute("xmlns", "http://www.w3.org/2000/svg");
        output.Attributes.SetAttribute("focusable", "false");
        output.Attributes.SetAttribute("role", "presentation");

        output.Content.SetHtmlContent(@"
            <g stroke=""none"" stroke-width=""1"" fill=""none"" fill-rule=""evenodd"">
                <path d=""M30,0 L19.787,8.857 L19.787234,30 L10.212766,23.8870644 L10.212,8.856 L0,0 L30,0 Z"" fill=""currentColor""></path>
            </g>
        ");
    }
}

