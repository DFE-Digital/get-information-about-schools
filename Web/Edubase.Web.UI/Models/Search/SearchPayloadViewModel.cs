using Edubase.Common;
using System.ComponentModel.DataAnnotations;

namespace Edubase.Web.UI.Models.Search
{
    public class SearchPayloadViewModel
    {
        [StringLength(200)]
        public string Text { get; set; }

        public string AutoSuggestValue { get; set; } = "";

        public int? AutoSuggestValueAsInt => AutoSuggestValue.ToInteger();
    }
}
