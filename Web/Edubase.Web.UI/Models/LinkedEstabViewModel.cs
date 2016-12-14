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
        public string Type { get; set; }
        public DateTime? LinkDate { get; set; }
        public string Name { get; set; }

        public LinkedEstabViewModel()
        {

        }

        public LinkedEstabViewModel(LinkedEstablishmentModel link)
        {
            Id = link.Id;
            Urn = link.Urn;
            Type = link.Type;
            LinkDate = link.LinkDate;
            Name = link.Name;
        }

        public LinkedEstabViewModel(EstablishmentLink link)
        {
            Id = link.Id;
            Urn = link.EstablishmentUrn;
            Type = link.LinkType?.Name;
            LinkDate = link.LinkEstablishedDate;
            Name = link.LinkName;
        }
    }
}