using System;
using Edubase.Services.Establishments.Models;

namespace Edubase.Web.UI.Models
{
    public class LinkedEstabViewModel
    {
        public int? Id { get; set; }
        public int? Urn { get; set; }
        public DateTime? LinkDate { get; set; }
        public DateTimeViewModel LinkDateEditable { get; set; } = new DateTimeViewModel();
        public string EstablishmentName { get; set; }
        public int? LinkTypeId { get; set; }
        public string LinkTypeName { get; set; }
        public string Address { get; set; }
        public bool IsNew => !Id.HasValue;


        public bool CreateReverseLink { get; set; }
        public bool ReverseLinkSameDate { get; set; }
        public DateTimeViewModel ReverseLinkDateEditable { get; set; } = new DateTimeViewModel();
        public int? ReverseLinkTypeId { get; set; }


        public LinkedEstabViewModel()
        {

        }

        public LinkedEstabViewModel(LinkedEstablishmentModel link)
        {
            Id = link.Id;
            Urn = link.Urn;
            LinkTypeId = link.LinkTypeId;
            LinkDate = link.LinkDate;
            EstablishmentName = link.EstablishmentName;
            LinkDateEditable = new DateTimeViewModel(link.LinkDate);
        }
        
    }
}
