using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Edubase.Web.UI.Views.TagHelpers;

[HtmlTargetElement("filter-skip-link")]
public class FilterSkipLinkTagHelper : TagHelper
{
    public string TargetId { get; set; } = "#results-container";

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "a";
        output.Attributes.SetAttribute("href", TargetId);
        output.Attributes.SetAttribute("class", "govuk-visually-hidden-focusable gias-filter-skip-link");
        output.Content.SetContent("Skip to results");
    }
}

