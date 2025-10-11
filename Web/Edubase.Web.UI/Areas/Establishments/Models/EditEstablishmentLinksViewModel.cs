using System.Collections.Generic;
using Edubase.Common;
using Edubase.Services.Domain;
using Edubase.Web.UI.Models;
using Edubase.Services.Groups.Models;

namespace Edubase.Web.UI.Areas.Establishments.Models
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

        public string TypeName { get; set; }
        public GroupModel LegalParentGroup
        {
            get
            {
                return UriHelper.TryDeserializeUrlToken<GroupModel>(LegalParentGroupToken);
            }
            set
            {
                LegalParentGroupToken = UriHelper.SerializeToUrlToken(value);
            }
        }

        public string LegalParentGroupToken { get; set; }

        public void HydrateStateToken()
        {
            StateToken = UriHelper.SerializeToUrlToken(this);
        }
    }

}