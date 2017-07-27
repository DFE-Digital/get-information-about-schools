using Edubase.Web.UI.Helpers.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Edubase.Web.UI.Models
{
    public class AgeRangeFilterViewModel
    {
        [BindAlias("a"), Display(Name = "From")]
        public int? From { get; set; }

        [BindAlias("b"), Display(Name = "To")]
        public int? To { get; set; }
    }
}