using Edubase.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models.Search
{
    public class SearchPayloadViewModel
    {
        public string Text { get; set; }
        public string AutoSuggestValue { get; set; }
        public int? AutoSuggestValueAsInt => AutoSuggestValue.ToInteger();
    }
}