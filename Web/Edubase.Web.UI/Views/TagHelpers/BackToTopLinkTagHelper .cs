using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Edubase.Web.UI.Views.TagHelpers;

[HtmlTargetElement("back-to-top-link")]
public class BackToTopLinkTagHelper : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "a";
        output.Attributes.SetAttribute("class", "back-to-top-link");
        output.Attributes.SetAttribute("href", "#top");

        output.Content.SetHtmlContent(@"
            <svg role=""presentation"" focusable=""false"" class=""back-to-top-icon"" xmlns=""http://www.w3.org/2000/svg"" width=""13"" height=""17"" viewBox=""0 0 13 17"">
                <path fill=""currentColor"" d=""M6.5 0L0 6.5 1.4 8l4-4v12.7h2V4l4.3 4L13 6.4z""></path>
            </svg>
            Back to top
        ");
    }
}
