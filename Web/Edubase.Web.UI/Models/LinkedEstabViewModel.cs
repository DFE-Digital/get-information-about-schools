using Edubase.Data.Entity;
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
        public string Type { get; set; }
        public DateTime? LinkDate { get; set; }
        public string Name { get; set; }

        public LinkedEstabViewModel()
        {

        }

        public LinkedEstabViewModel(EstablishmentLink link)
        {
            Id = link.Id;
            Urn = link.LinkedEstablishmentUrn;
            Type = link.LinkType.Name;
            LinkDate = link.LinkEstablishedDate;
            Name = link.LinkedEstablishment.Name;
        }
    }
}