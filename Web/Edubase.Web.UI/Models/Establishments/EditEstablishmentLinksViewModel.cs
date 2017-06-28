using Edubase.Services.Establishments.Models;
using System.Collections.Generic;
using System;
using System.Web.Mvc;
using Edubase.Common;
using Newtonsoft.Json;
using Edubase.Services.Domain;

namespace Edubase.Web.UI.Models.Establishments
{
    public class EditEstablishmentLinksViewModel : IEstablishmentPageViewModel
    {
        public IEnumerable<LinkedEstabViewModel> Links { get; set; }
        public string SelectedTab { get; set; }
        public int? Urn { get; set; }
        public string Name { get; set; }
        public TabDisplayPolicy TabDisplayPolicy { get; set; }
        public string Layout { get; set; }
        public IEnumerable<SelectListItem> LinkTypes => LinkTypeList?.ToSelectList(ActiveRecord?.LinkTypeId);
        public IEnumerable<SelectListItem> ReverseLinkTypes => LinkTypeList?.ToSelectList(ActiveRecord?.ReverseLinkTypeId);
        public IEnumerable<LookupDto> LinkTypeList { get; set; }
        public LinkedEstabViewModel ActiveRecord { get; set; }
        public string StateToken { get; set; }
        public string Act { get; set; }
        public bool IsNew => (ActiveRecord?.IsNew).GetValueOrDefault();
        
        public void HydrateStateToken()
        {
            StateToken = UriHelper.SerializeToUrlToken(this);
        }
    }

}