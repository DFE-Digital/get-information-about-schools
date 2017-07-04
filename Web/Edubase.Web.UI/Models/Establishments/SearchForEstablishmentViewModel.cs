using Edubase.Web.UI.Helpers.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Edubase.Web.UI.Models.Establishments
{
    public class SearchForEstablishmentViewModel : IEstablishmentPageViewModel
    {
        public string SelectedTab { get; set; }
        public int? Urn { get; set; }
        public string Name { get; set; }
        public TabDisplayPolicy TabDisplayPolicy { get; set; }
        public string Layout { get; set; }
        
        public string SearchUrn { get; set; }

        public const string BIND_ALIAS_DOSEARCH = "s";

        [BindAlias(BIND_ALIAS_DOSEARCH)]
        public bool DoSearch { get; set; }
    }
}