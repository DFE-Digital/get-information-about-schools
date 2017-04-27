#if (!TEXAPI)
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
            HeadteacherDetails = true;
            AgeRange = true;
            GenderId = true;
            EstablishmentNumber = true;
            AdmissionsPolicyId = true;
            Contact_WebsiteAddress = true;
            ReligiousCharacterId = true;
            ReligiousEthosId = true;
            ProvisionBoardingId = true;
            Section41ApprovedId = true;
            Capacity = true;
            CloseDate = IsSchoolClosed;
            ReasonEstablishmentClosedId = IsSchoolClosed;
            ProvisionSpecialClassesId = true;
            Contact_EmailAddress= IsUserLoggedIn;
            ContactAlt_EmailAddress = IsUserLoggedIn;
            LastChangedDate = IsUserLoggedIn;
            InspectorateId = true;
            ProprietorName = true;
            SENStat = true;
            SENNoStat = true;

        }
    }
}

#endif