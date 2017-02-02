using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Areas.Groups.Models.CreateEdit
{
    public class GroupLinkedEstablishmentsViewModel
    {
        public LinkedEstablishmentSearchViewModel LinkedEstablishmentSearch { get; set; } = new LinkedEstablishmentSearchViewModel();

        public List<EstablishmentGroupViewModel> Establishments { get; set; } = new List<EstablishmentGroupViewModel>();
        
    }
}