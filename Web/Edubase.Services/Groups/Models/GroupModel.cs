using Edubase.Common.Reflection;
using Edubase.Services.Domain;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Edubase.Services.Groups.Models
{

    public class GroupModel
    {
        public int? GroupUId { get; set; }
        public string Name { get; set; }
        public string CompaniesHouseNumber { get; set; }
        public int? GroupTypeId { get; set; }
        public DateTime? ClosedDate { get; set; }
        public int? StatusId { get; set; }
        public DateTime? OpenDate { get; set; }
        [IgnoreChangesAttribute]
        public int? HeadTitleId { get; set; }
        public string HeadFirstName { get; set; }
        public string HeadLastName { get; set; }
        public AddressDto Address { get; set; }
        public string ManagerEmailAddress { get; set; }

        [IgnoreChangesAttribute]
        public string DelegationInformation { get; set; }
        public string GroupId { get; set; }
        public int? LocalAuthorityId { get; set; }
        public string CorporateContact { get; set; }

        [DisplayName("UKPRN"), JsonProperty("UKPRN")]
        public string UKPRN { get; set; }

        [IgnoreChangesAttribute]
        public bool ConfirmationUpToDateGovernanceRequired { get; set; }

        [IgnoreChangesAttribute]
        public bool UrgentConfirmationUpToDateGovernanceRequired { get; set; }

        [IgnoreChangesAttribute]
        public DateTime? ConfirmationUpToDateGovernance_LastConfirmationDate { get; set; }

    }
}
