using Edubase.Web.UI.Helpers.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Edubase.Web.UI.Models
{
    public class AgeRangeFilterViewModel
    {
        [BindAlias("4"), Display(Name = "From")]
        public int? From { get; set; }

        [BindAlias("3"), Display(Name = "To")]
        public int? To { get; set; }
    }
}
