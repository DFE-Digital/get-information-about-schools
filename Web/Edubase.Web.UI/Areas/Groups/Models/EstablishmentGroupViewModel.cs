using System;
using System.ComponentModel.DataAnnotations;
using Edubase.Common;
using Edubase.Common.Spatial;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Areas.Groups.Models
{
    public class EstablishmentGroupViewModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int Urn { get; set; }
        public int? UKPRN { get; set; }
        public string Address { get; set; }
        public string TypeName { get; set; }
        public string HeadFirstName { get; set; }
        public string HeadLastName { get; set; }
        public DateTime? JoinedDate { get; set; }

        public string PhaseName { get; set; }
        public string LAESTAB { get; set; }
        public string LocalAuthorityName { get; set; }
        public string StatusName { get; set; }
        public LatLon Location { get; set; }

        public double? Latitude => Location?.Latitude;
        public double? Longitude => Location?.Longitude;

        [Display(Name = "Joined date")]
        public DateTimeViewModel JoinedDateEditable { get; set; } = new DateTimeViewModel();

        public string HeadTitleName { get; set; }
        public bool EditMode { get; set; }

        /// <summary>
        /// Where this is a Children's Centre establishment; this flag denotes whether this establishment is the 'lead centre'
        /// </summary>
        public bool CCIsLeadCentre { get; set; }

        public string HeadFullName => StringUtil.ConcatNonEmpties(" ", HeadTitleName, HeadFirstName, HeadLastName);

        public EstablishmentGroupViewModel SetEditMode(bool flag = true)
        {
            EditMode = flag;
            return this;
        }
    }
}
