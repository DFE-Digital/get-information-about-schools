using System.Collections.Generic;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Views.TagHelpers.Models;

public class SelectorViewModel
{
    public SelectorViewModel(
        string label,
        string name,
        IEnumerable<LookupItemViewModel> items,
        List<int> selectedItems,
        string additionalClassName = "")
    {
        Label = label;
        Name = name;
        Items = items;
        SelectedItems = selectedItems;
        AdditionalClassName = additionalClassName;
    }

    public string Label { get; }
    public string Name { get; }
    public IEnumerable<LookupItemViewModel> Items;
    public List<int> SelectedItems;
    public string AdditionalClassName { get; }
}
