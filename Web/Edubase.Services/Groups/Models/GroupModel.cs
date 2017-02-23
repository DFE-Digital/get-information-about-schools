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
        public string Address { get; set; }
        public string ManagerEmailAddress { get; set; }

        [DisplayName("Group Id")]
        public string GroupId { get; set; }

        public int EstablishmentCount { get; set; }
        public int? LocalAuthorityId { get; set; }
    }
}
