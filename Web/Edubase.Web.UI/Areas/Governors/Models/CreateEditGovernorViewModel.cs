using Edubase.Web.UI.Areas.Groups.Models.CreateEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Edubase.Services.Groups.Models;
using Edubase.Services.Enums;
using System.Web.Mvc;
using System.ComponentModel;
using Edubase.Web.UI.Models;
using Edubase.Services.Governors.DisplayPolicies;
using Edubase.Services.Nomenclature;
using Edubase.Web.UI.Models.Establishments;

namespace Edubase.Web.UI.Areas.Governors.Models
{
    public class CreateEditGovernorViewModel : IGroupPageViewModel, IEstablishmentPageViewModel
    {
        private static readonly IEnumerable<SelectListItem> _titles = new[] { "Mr", "Mrs", "Ms", "Miss", "Dr", "Prof", "Rev", "Sir" }.Select(x => new SelectListItem { Text = x, Value = x });
        
        public eLookupGovernorRole GovernorRole { get; set; }
        public string GovernorRoleName { get; set; }
        public IEnumerable<SelectListItem> Titles { get; set; }
        public IEnumerable<SelectListItem> PreviousTitles { get; set; }
        public IEnumerable<SelectListItem> AppointingBodies { get; set; }
        public IEnumerable<SelectListItem> Nationalities { get; set; }

        public int? GID { get; set; }

        public int? EstablishmentUrn { get; set; }
        public int? GroupUId { get; set; }
        public string ParentControllerName => EstablishmentUrn.HasValue ? "Establishment" : "Group";
        public string ParentAreaName => EstablishmentUrn.HasValue ? "" : "Groups";

        [DisplayName("Title")]
        public string GovernorTitle { get; set; }

        [DisplayName("First name")]
        public string FirstName { get; set; }

        [DisplayName("Second name")]
        public string MiddleName { get; set; }

        [DisplayName("Last name")]
        public string LastName { get; set; }

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

        [DisplayName("Nationality")]
        public int? NationalityId { get; set; }

        [DisplayName("Home postcode")]
        public string PostCode { get; set; }

        [DisplayName("Telephone")]
        public string TelephoneNumber { get; set; }


        [DisplayName("Previous title")]
        public string PreviousTitle { get; set; }

        [DisplayName("Previous first name")]
        public string PreviousFirstName { get; set; }

        [DisplayName("Previous second name")]
        public string PreviousMiddleName { get; set; }

        [DisplayName("Previous last name")]
        public string PreviousLastName { get; set; }

        public GovernorDisplayPolicy DisplayPolicy { get; internal set; }

        public ReplaceGovernorViewModel ReplaceGovernorViewModel { get; set; } = new ReplaceGovernorViewModel();


        public string Layout { get; set; }
        
        public string ListOfEstablishmentsPluralName { get; set; }

        public string GroupName { get; set; }

        public int? GroupTypeId { get; set; }

        public string SelectedTabName { get; set; }

        public CreateEditGovernorViewModel()
        {

        }

        public string FormPostRouteName
        {
            get
            {
                var part1 = EstablishmentUrn.HasValue ? "Estab" : "Group";
                var part2 = ReplaceGovernorViewModel.GID.HasValue ? "Replace" : (GID.HasValue ? "Edit" : "Add");
                return string.Concat(part1, part2, "Governor");
            }
        }

        string IEstablishmentPageViewModel.SelectedTab { get; set; }

        int? IEstablishmentPageViewModel.Urn { get; set; }

        string IEstablishmentPageViewModel.Name { get; set; }

        TabDisplayPolicy IEstablishmentPageViewModel.TabDisplayPolicy { get; set; }

        public IEnumerable<SelectListItem> GetTitles() => new[] { "Mr", "Mrs", "Ms", "Miss", "Dr", "Prof", "Rev", "Sir" }.Select(x => new SelectListItem { Text = x, Value = x });

    }
}