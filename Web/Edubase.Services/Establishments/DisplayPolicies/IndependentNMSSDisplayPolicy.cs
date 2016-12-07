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
            GenderOfEntry = true;
            LAESTAB = true;
            AdmissionsPolicy = true;
            WebsiteAddress = true;
            ReligiousCharacter = true;
            ReligiousEhtos = true;
            BoardingProvision = true;
            Section41Approved = true;
            Capacity = true;
            CloseDate = IsSchoolClosed;
            ReasonEstablishmentClosed = IsSchoolClosed;
            SpecialClasses = true;
            MainEmailAddress = IsUserLoggedIn;
            AlternativeEmailAddress = MainEmailAddress;
            LastChangedDate = IsUserLoggedIn;
            Inspectorate = true;
            Proprietor = true;
            SENStat = true;
            SENNoStat = true;

        }
    }
}