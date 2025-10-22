using System.Collections.Generic;

namespace Edubase.Web.UI.Areas.Groups.Models.CreateEdit
{
    public class GroupLinkedEstablishmentsViewModel
    {
        public LinkedEstablishmentSearchViewModel LinkedEstablishmentSearch { get; set; } = new LinkedEstablishmentSearchViewModel();

        public List<EstablishmentGroupViewModel> Establishments { get; set; } = new List<EstablishmentGroupViewModel>();
        
    }
}
