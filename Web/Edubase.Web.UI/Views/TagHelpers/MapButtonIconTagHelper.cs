using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Edubase.Web.UI.Views.TagHelpers;

[HtmlTargetElement("map-button-icon")]
public class MapButtonIconTagHelper : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "svg";
        output.Attributes.SetAttribute("width", "20px");
        output.Attributes.SetAttribute("height", "30px");
        output.Attributes.SetAttribute("viewBox", "0 0 20 30");
        output.Attributes.SetAttribute("xmlns", "http://www.w3.org/2000/svg");
        output.Attributes.SetAttribute("role", "presentation");
        output.Attributes.SetAttribute("focusable", "false");

        output.Content.SetHtmlContent(@"
            <g stroke=""none"" stroke-width=""1"" fill=""none"" fill-rule=""evenodd"">
                <path d=""M10,0 C15.5228475,0 20,4.96379951 20,11.0869565 C20,17.2101135 10,30 9.97821351,30 C9.97242216,30 9.2646317,29.0962442 8.22846607,27.654196 L7.92380612,27.2278506 C6.77632342,25.6131918 5.3074999,23.4459539 3.94691638,21.146335 L3.57885453,20.5161997 C1.63361341,17.1413409 0,13.5672227 0,11.0869565 C0,4.96379951 4.4771525,0 10,0 Z M10,4.58937198 C6.93175139,4.58937198 4.44444444,7.34703838 4.44444444,10.7487923 C4.44444444,14.1505462 6.93175139,16.9082126 10,16.9082126 C13.0682486,16.9082126 15.5555556,14.1505462 15.5555556,10.7487923 C15.5555556,7.34703838 13.0682486,4.58937198 10,4.58937198 Z"" fill=""currentColor""></path>
            </g>
        ");
    }
}
