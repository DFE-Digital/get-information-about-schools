using Edubase.Common.Reflection;
using Edubase.Services.Domain;
using System;
using System.ComponentModel;

namespace Edubase.Services.Groups.Models
{

    public class GroupModel
    {
        public int? GroupUID { get; set; }
        public string Name { get; set; }
        public string CompaniesHouseNumber { get; set; }
        public int? GroupTypeId { get; set; }
        public DateTime? ClosedDate { get; set; }
        public int? StatusId { get; set; }
        public DateTime? OpenDate { get; set; }
        public PersonDto Head { get; set; } = new PersonDto();
        public AddressDto Address { get; set; }
        public string ManagerEmailAddress { get; set; }
        public string DelegationInformation { get; set; }

        [DisplayName("Group Id")]
        public string GroupId { get; set; }

        [IgnoreChanges]
        public int EstablishmentCount { get; set; }
        public int? LocalAuthorityId { get; set; }
    }
}
