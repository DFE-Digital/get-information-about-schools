using System.Collections.Generic;

namespace Edubase.Web.UI.Models.Search;

public class SelectorNestedViewModel
{
    public string Label { get; set; }                     
    public string Name { get; set; }                      
    public IEnumerable<HeirarchicalLookupItemViewModel> Items { get; set; } 
    public List<int> SelectedItems { get; set; }          
    public string AdditionalClassName { get; set; } = "";
    public bool IsExtraFilter { get; set; } = false;
    public bool FilterRefine { get; set; } = false;
    public string SelectedExtraSearchFilters { get; set; }
}
