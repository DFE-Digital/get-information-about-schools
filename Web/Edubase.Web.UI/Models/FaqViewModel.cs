using Edubase.Data.Entity;
using System.Collections.Generic;
using System.Linq;

namespace Edubase.Web.UI.Models
{
    public class FaqViewModel
    {   
        public bool UserCanEdit { get; set; }
        public IEnumerable<FaqItem> Items { get; }

        public FaqViewModel(IEnumerable<FaqItem> items)
        {
            Items = items.OrderBy(x => x.DisplayOrder);
        }
    }
}