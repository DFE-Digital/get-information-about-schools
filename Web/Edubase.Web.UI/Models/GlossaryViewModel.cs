using System.Collections.Generic;
using System.Linq;
using Edubase.Data.Entity;

namespace Edubase.Web.UI.Models
{
    public class GlossaryViewModel
    {   
        public bool UserCanEdit { get; set; }
        public IEnumerable<IGrouping<char, GlossaryItem>> Items { get; }

        public GlossaryViewModel(IEnumerable<GlossaryItem> items)
        {
            Items = items.OrderBy(x => x.Title).GroupBy(x => char.ToUpper(x.Title[0]));
        }
    }
}
