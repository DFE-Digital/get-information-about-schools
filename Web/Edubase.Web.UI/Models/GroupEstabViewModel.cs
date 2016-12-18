using Edubase.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models
{
    public class GroupEstabViewModel
    {
        public int Urn { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string HeadteacherName { get; set; }
        public string Type { get; set; }

        public GroupEstabViewModel()
        {

        }

        public GroupEstabViewModel(Establishment establishment)
        {
            Urn = establishment.Urn;
            Name = establishment.Name;
            Address = establishment.Address.ToString();
            HeadteacherName = establishment.HeadteacherFullName;
            Type = establishment.EstablishmentType?.Name;
        }
    }
}