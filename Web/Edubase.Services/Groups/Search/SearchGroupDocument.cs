using Edubase.Common;
using Edubase.Services.Domain;
using System;

namespace Edubase.Services.Groups.Models
{

    public class SearchGroupDocument
    {
        public int GroupUID { get; set; }
        public string Name { get; set; }
        public string NameDistilled { get; set; }
        public string CompaniesHouseNumber { get; set; }
        public int? GroupTypeId { get; set; }
        public DateTime? ClosedDate { get; set; }
        public int? StatusId { get; set; }
        public DateTime? OpenDate { get; set; }
        public string Address { get; set; }
        public string ManagerEmailAddress { get; set; }
        public string GroupId { get; set; }
        public int EstablishmentCount { get; set; }
        public int? LocalAuthorityId { get; set; }

        public string Head_Title { get; set; }
        public string Head_FirstName { get; set; }
        public string Head_MiddleName { get; set; }
        public string Head_LastName { get; set; }
        public string Head_FullName => StringUtil.ConcatNonEmpties(" ", Head_Title, Head_FirstName, Head_LastName);
    }
}
