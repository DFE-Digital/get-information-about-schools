using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Edubase.Services.Enums;
using Edubase.Services.Governors.DisplayPolicies;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Areas.Governors.Models
{
    public class GovernorViewModel
    {
        [DisplayName("Governance role identifier (GID)")]
        public int? GID { get; set; }

        public eLookupGovernorRole GovernorRole { get; set; }

        [DisplayName("Title")]
        public int? GovernorTitleId { get; set; }

        private string GovernorTitle => Titles
            .Where(x => x.Value == GovernorTitleId.ToString())
            .Select(x => x.Text)
            .FirstOrDefault() ?? "";

        [DisplayName("First name")]
        public string FirstName { get; set; }

        [DisplayName("Second name")]
        public string MiddleName { get; set; }

        [DisplayName("Last name")]
        public string LastName { get; set; }

        /// <summary>
        /// See also the browser-side JavaScript implementation for full name,
        /// whose implementation should be kept synchronised with the server-side C# implementation:
        ///   - Web/Edubase.Web.UI/Assets/Scripts/Entry/add-edit-governor.js
        ///
        /// Notes:
        /// - Do not include the title in the "full name", if the title is not provided
        /// - Join the name parts with only a single space between each name part
        /// </summary>
        [DisplayName("Name")]
        public string FullName => string.Join(
            " ",
            new[]
                {
                    GovernorTitle
                        .Replace("Not-applicable", string.Empty)
                        .Replace("Not recorded", string.Empty),
                    FirstName,
                    MiddleName,
                    LastName,
                }
                .Where(part => !string.IsNullOrWhiteSpace(part))
                .Select(part => part.Trim())
        );

        [DisplayName("Appointing body")]
        public int? AppointingBodyId { get; set; }

        [DisplayName("Date of appointment")]
        public DateTimeViewModel AppointmentStartDate { get; set; } = new DateTimeViewModel();

        [DisplayName("Date appointment ended")]
        public DateTimeViewModel AppointmentEndDate { get; set; } = new DateTimeViewModel();

        [DisplayName("Email address")]
        public string EmailAddress { get; set; }

        [DisplayName("Date of birth")]
        public DateTimeViewModel DOB { get; set; } = new DateTimeViewModel();

        [DisplayName("Home postcode")]
        public string PostCode { get; set; }

        [DisplayName("Telephone number")]
        public string TelephoneNumber { get; set; }


        [DisplayName("Previous title")]
        public int? PreviousTitleId { get; set; }

        [DisplayName("Previous first name")]
        public string PreviousFirstName { get; set; }

        [DisplayName("Previous second name")]
        public string PreviousMiddleName { get; set; }

        [DisplayName("Previous last name")]
        public string PreviousLastName { get; set; }

        public GovernorEditPolicy EditPolicy { get; internal set; }

        public IEnumerable<SelectListItem> Titles { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> PreviousTitles { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> AppointingBodies { get; set; } = new List<SelectListItem>();
    }
}
