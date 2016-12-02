using Edubase.Common;
using Edubase.Services.Enums;

namespace Edubase.Services.Establishments.DisplayPolicies
{
    public class IndependentNMSSDisplayPolicy : EstablishmentDisplayPolicy
    {
        protected override bool IsMatchInternal(eLookupEstablishmentType type, eLookupEstablishmentTypeGroup typeGroup)
        {
            return typeGroup.OneOfThese(eLookupEstablishmentTypeGroup.IndependentSchools, eLookupEstablishmentTypeGroup.SpecialSchools)
            &&
            type.OneOfThese(eLookupEstablishmentType.OtherIndependentSchool, 
                            eLookupEstablishmentType.OtherIndependentSpecialSchool,
                            eLookupEstablishmentType.NonmaintainedSpecialSchool);
        }

        protected override void ConfigureInternal()
        {
            var isUserLoggedIn = Principal.Identity.IsAuthenticated;
            var isSchoolClosed = Establishment.StatusId == (int)eLookupEstablishmentStatus.Closed;

            HeadteacherDetails = true;
            AgeRange = true;
            GenderOfEntry = true;
            LAESTAB = true;
            AdmissionsPolicy = true;
            WebsiteAddress = true;
            ReligiousCharacter = true;
            ReligiousEhtos = true;
            BoardingProvision = true;
            Section41Approved = true;
            Capacity = true;
            CloseDate = isSchoolClosed;
            ReasonEstablishmentClosed = isSchoolClosed;
            SpecialClasses = true;
            MainEmailAddress = isUserLoggedIn;
            AlternativeEmailAddress = MainEmailAddress;
            LastChangedDate = isUserLoggedIn;
            Inspectorate = true;
            Proprietor = true;
            SENStat = true;
            SENNoStat = true;

        }
    }
}