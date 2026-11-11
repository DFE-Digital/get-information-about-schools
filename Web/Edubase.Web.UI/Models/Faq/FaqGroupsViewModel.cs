using System.Collections.Generic;
using System.Linq;
using Edubase.Data.Entity;

namespace Edubase.Web.UI.Models.Faq
{
    public class FaqGroupsViewModel
    {
        public IEnumerable<FaqGroup> Groups { get; set; }

        public FaqGroupsViewModel(IEnumerable<FaqGroup> items)
        {
            Groups = items.OrderBy(x => x.DisplayOrder);
        }
    }
}
