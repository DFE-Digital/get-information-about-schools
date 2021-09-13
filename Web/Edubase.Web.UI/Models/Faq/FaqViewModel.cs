using Edubase.Data.Entity;
using System.Collections.Generic;
using System.Linq;

namespace Edubase.Web.UI.Models
{
    public class FaqViewModel
    {   
        public bool UserCanEdit { get; set; }
        public IEnumerable<FaqItem> Items { get; }
        public IEnumerable<FaqGroup> Groups { get; }
        public FaqViewModel(IEnumerable<FaqItem> items, IEnumerable<FaqGroup> groups)
        {
            Items = items.OrderBy(x => x.GroupId).ThenBy(x => x.DisplayOrder);
            Groups = groups.OrderBy(x => x.DisplayOrder);
        }
    }
}
