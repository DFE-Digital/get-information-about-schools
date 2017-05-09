using Edubase.Data.Entity;
using Edubase.Services.Establishments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models
{
    public class LinkedEstabViewModel
    {
        public int? Id { get; set; }
        public int? Urn { get; set; }
        public DateTime? LinkDate { get; set; }
        public string EstablishmentName { get; set; }
        public int? LinkTypeId { get; set; }
        public string LinkTypeName { get; set; }


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
        }
        
    }
}