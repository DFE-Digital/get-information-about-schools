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
            var isUserLoggedIn = Principal.Identity.IsAuthenticated;
            var isSchoolClosed = Establishment.StatusId == (int)eLookupEstablishmentStatus.Closed;

            HeadteacherDetails = true;
            AgeRange = true;
            GenderOfEntry = true;
            WebsiteAddress = true;
            CloseDate = isSchoolClosed;
            ReasonEstablishmentClosed = isSchoolClosed;
            MainEmailAddress = isUserLoggedIn;
            AlternativeEmailAddress = MainEmailAddress;
            LastChangedDate = isUserLoggedIn;
            FurtherEducationType = true;
        }
    }
}