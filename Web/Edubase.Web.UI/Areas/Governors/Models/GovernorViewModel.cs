using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;
using Edubase.Services.Enums;
using Edubase.Services.Governors.DisplayPolicies;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Areas.Governors.Models
{
    public class GovernorViewModel
    {
        public int? GID { get; set; }

        public eLookupGovernorRole GovernorRole { get; set; }

        [DisplayName("Title")]
        public int? GovernorTitleId { get; set; }

        [DisplayName("First name")]
        public string FirstName { get; set; }

        [DisplayName("Second name")]
        public string MiddleName { get; set; }

        [DisplayName("Last name")]
        public string LastName { get; set; }

        public bool? IsOriginalSignatoryMember { get; set; }    

        [DisplayName("Appointing body")]
        public int? AppointingBodyId { get; set; }

        [DisplayName("Date of appointment")]
        public DateTimeViewModel AppointmentStartDate { get; set; } = new DateTimeViewModel();

        [DisplayName("Date term ends")]
        public DateTimeViewModel AppointmentEndDate { get; set; } = new DateTimeViewModel();

        [DisplayName("Email address")]
        public string EmailAddress { get; set; }

        [DisplayName("Date of birth")]
        public DateTimeViewModel DOB { get; set; } = new DateTimeViewModel();

        [DisplayName("Home postcode")]
        public string PostCode { get; set; }

        [DisplayName("Telephone")]
        public string TelephoneNumber { get; set; }


        [DisplayName("Previous title")]
        public int? PreviousTitleId { get; set; }

        [DisplayName("Previous first name")]
        public string PreviousFirstName { get; set; }

        [DisplayName("Previous second name")]
        public string PreviousMiddleName { get; set; }

        [DisplayName("Previous last name")]
        public string PreviousLastName { get; set; }

        public GovernorDisplayPolicy DisplayPolicy { get; internal set; }

        public IEnumerable<SelectListItem> YesNoSelect { get; set; } = new[] { new SelectListItem { Value = "true", Text = "Yes", }, new SelectListItem { Value = "false", Text = "No", Selected = true } };
        public IEnumerable<SelectListItem> Titles { get; set; }
        public IEnumerable<SelectListItem> PreviousTitles { get; set; }
        public IEnumerable<SelectListItem> AppointingBodies { get; set; }
    }
}
