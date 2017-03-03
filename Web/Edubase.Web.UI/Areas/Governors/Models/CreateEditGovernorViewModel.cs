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

namespace Edubase.Web.UI.Areas.Governors.Models
{
    public class CreateEditGovernorViewModel : IGroupPageViewModel
    {

        #region IGroupPageViewModel wiring

        private GroupEditorViewModelBase _groupEditorViewModel;
        public string PageTitle => _groupEditorViewModel.PageTitle;
        public string ListOfEstablishmentsPluralName => _groupEditorViewModel.ListOfEstablishmentsPluralName;

        public string GroupName
        {
            get { return _groupEditorViewModel.GroupName; }
            set { _groupEditorViewModel.GroupName = value; }
        }

        public int? GroupTypeId
        {
            get { return _groupEditorViewModel.GroupTypeId; }
            set { _groupEditorViewModel.GroupTypeId = value; }
        }
        public int? GroupUId
        {
            get { return _groupEditorViewModel.GroupUId; }
            set { _groupEditorViewModel.GroupUId = value; }
        }

        #endregion

        private static readonly IEnumerable<SelectListItem> _titles = new[] { "Mr", "Mrs", "Ms", "Miss", "Dr", "Prof", "Rev", "Sir" }.Select(x => new SelectListItem { Text = x, Value = x });

        public eLookupGovernorRole GovernorRole { get; set; }
        public string GovernorRoleName { get; set; }
        public IEnumerable<SelectListItem> Titles { get; set; }
        public IEnumerable<SelectListItem> PreviousTitles { get; set; }
        public IEnumerable<SelectListItem> AppointingBodies { get; set; }
        public IEnumerable<SelectListItem> Nationalities { get; set; }

        public int? GID { get; set; }
        
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

        public CreateEditGovernorViewModel(GroupModel domainModel)
        {
            _groupEditorViewModel = new GroupEditorViewModelBase(domainModel);
        }

        public CreateEditGovernorViewModel()
        {
            _groupEditorViewModel = new GroupEditorViewModelBase();
        }


        public IEnumerable<SelectListItem> GetTitles() => new[] { "Mr", "Mrs", "Ms", "Miss", "Dr", "Prof", "Rev", "Sir" }.Select(x => new SelectListItem { Text = x, Value = x });

    }
}