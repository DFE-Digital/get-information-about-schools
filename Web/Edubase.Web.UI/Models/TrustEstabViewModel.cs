using Edubase.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Edubase.Web.UI.Models
{
    public class TrustEstabViewModel
    {
        public int Urn { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string HeadteacherName { get; set; }
        public string Type { get; set; }

        public TrustEstabViewModel()
        {

        }

        public TrustEstabViewModel(Establishment establishment)
        {
            Urn = establishment.Urn;
            Name = establishment.Name;
            Address = Helpers.SchoolDetailsHelpers.GetAddress(establishment);
            HeadteacherName = establishment.HeadteacherFullName;
            Type = establishment.EstablishmentType?.Name;
        }
    }
}