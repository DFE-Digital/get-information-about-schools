using Edubase.Common;
using Edubase.Services.Domain;
using System;

namespace Edubase.Services.Groups.Models
{
    public class SearchGroupDocument
    {
        public int GroupUId { get; set; }
        public string Name { get; set; }
        public string CompaniesHouseNumber { get; set; }
        public int? GroupTypeId { get; set; }
        public DateTime? ClosedDate { get; set; }
        public int? StatusId { get; set; }
        public DateTime? OpenDate { get; set; }
        public string ManagerEmailAddress { get; set; }
        public string GroupId { get; set; }
        public int EstablishmentCount { get; set; }
        public int? LocalAuthorityId { get; set; }
        public AddressDto Address { get; set; }
    }
}
