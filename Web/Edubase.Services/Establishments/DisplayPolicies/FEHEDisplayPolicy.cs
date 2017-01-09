using Edubase.Common;
using Edubase.Services.Enums;

namespace Edubase.Services.Establishments.DisplayPolicies
{
    public class FEHEDisplayPolicy : EstablishmentDisplayPolicy
    {
        protected override bool IsMatchInternal(eLookupEstablishmentType type, eLookupEstablishmentTypeGroup typeGroup)
        {
            return typeGroup.OneOfThese(eLookupEstablishmentTypeGroup.Colleges, eLookupEstablishmentTypeGroup.Universities)
            &&
            type.OneOfThese(eLookupEstablishmentType.FurtherEducation, eLookupEstablishmentType.HigherEducationInstitutions);
        }

        protected override void ConfigureInternal()
        {
            HeadteacherDetails = true;
            AgeRange = true;
            GenderId = true;
            Contact_WebsiteAddress = true;
            CloseDate = IsSchoolClosed;
            ReasonEstablishmentClosedId = IsSchoolClosed;
            Contact_EmailAddress = IsUserLoggedIn;
            ContactAlt_EmailAddress = IsUserLoggedIn;
            LastChangedDate = IsUserLoggedIn;
            FurtherEducationTypeId = true;
        }
    }
}