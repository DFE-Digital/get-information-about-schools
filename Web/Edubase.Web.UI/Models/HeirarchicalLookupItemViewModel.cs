using System.Collections.Generic;

namespace Edubase.Web.UI.Models
{
    public class HeirarchicalLookupItemViewModel : LookupItemViewModel
    {
        public List<HeirarchicalLookupItemViewModel> ChildItems { get; set; }
    }
}