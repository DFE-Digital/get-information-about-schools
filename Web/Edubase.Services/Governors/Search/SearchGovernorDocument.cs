using Edubase.Common;
using System;

namespace Edubase.Services.Governors.Search
{
    public class SearchGovernorDocument
    {
        public int Id { get; set; }
        public int? EstablishmentUrn { get; set; }

        public string Person_Title { get; set; }
        public string Person_FirstName { get; set; }
        public string Person_MiddleName { get; set; }
        public string Person_LastName { get; set; }

        public string Person_TitleDistilled { get; set; }
        public string Person_FirstNameDistilled { get; set; }
        public string Person_MiddleNameDistilled { get; set; }
        public string Person_LastNameDistilled { get; set; }

        public string PreviousPerson_Title { get; set; }
        public string PreviousPerson_FirstName { get; set; }
        public string PreviousPerson_MiddleName { get; set; }
        public string PreviousPerson_LastName { get; set; }

        public string PreviousPerson_TitleDistilled { get; set; }
        public string PreviousPerson_FirstNameDistilled { get; set; }
        public string PreviousPerson_MiddleNameDistilled { get; set; }
        public string PreviousPerson_LastNameDistilled { get; set; }

        public DateTime? AppointmentStartDate { get; set; }
        public DateTime? AppointmentEndDate { get; set; }
        public int? RoleId { get; set; }
        public int? AppointingBodyId { get; set; }
        public string EmailAddress { get; set; }
        public DateTime? DOB { get; set; }
        public string Nationality { get; set; }
        public string PostCode { get; set; }
        public int? GroupUID { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
        public bool IsDeleted { get; set; }

        public string GetFullName() => StringUtil.ConcatNonEmpties(" ", Person_Title, Person_FirstName, Person_MiddleName, Person_LastName);
    }
}
