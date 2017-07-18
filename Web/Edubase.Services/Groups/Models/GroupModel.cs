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

        public int? HeadTitleId { get; set; }
        public string HeadFirstName { get; set; }
        public string HeadLastName { get; set; }
        

        public AddressDto Address { get; set; }
        public string ManagerEmailAddress { get; set; }
        
        public string DelegationInformation { get; set; }
        
        public string GroupId { get; set; }
        
        public int? LocalAuthorityId { get; set; }

        public bool ConfirmationUpToDateGovernanceRequired { get; set; }
        public DateTime? ConfirmationUpToDateGovernance_LastConfirmationDate { get; set; }
    }
}
