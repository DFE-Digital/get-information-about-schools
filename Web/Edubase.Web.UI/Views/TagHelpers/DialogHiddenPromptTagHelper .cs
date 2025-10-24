using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Edubase.Web.UI.Views.TagHelpers;

[HtmlTargetElement("dialog-hidden-prompt")]
public class DialogHiddenPromptTagHelper : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "span";
        output.Attributes.SetAttribute("class", "govuk-visually-hidden");
        output.Content.SetContent("Opens a dialog");
    }
}
