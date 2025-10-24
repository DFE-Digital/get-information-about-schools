using System.Collections.Generic;
using Edubase.Web.UI.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Edubase.Web.UI.Views.TagHelpers;

[HtmlTargetElement("selector")]
public class SelectorTagHelper : TagHelper
{
    public string Label { get; set; }
    public string Name { get; set; }
    public IEnumerable<LookupItemViewModel> Items { get; set; } = [];
    public List<int> SelectedItems { get; set; } = [];
    public string AdditionalClassName { get; set; } = "";

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "div";
        output.Attributes.SetAttribute("class", $"selector-wrapper {AdditionalClassName}".Trim());

        TagBuilder labelTag = new("label");
        labelTag.InnerHtml.Append(Label);

        TagBuilder selectTag = new("select");
        selectTag.Attributes["name"] = Name;
        selectTag.Attributes["id"] = Name;
        selectTag.Attributes["multiple"] = "multiple";
        selectTag.AddCssClass("govuk-select");

        foreach (var item in Items)
        {
            TagBuilder option = new("option");
            option.Attributes["value"] = item.Id.ToString();
            if (SelectedItems.Contains(item.Id))
            {
                option.Attributes["selected"] = "selected";
            }
            option.InnerHtml.Append(item.Name);
            selectTag.InnerHtml.AppendHtml(option);
        }

        output.Content.AppendHtml(labelTag);
        output.Content.AppendHtml(selectTag);
    }
}

