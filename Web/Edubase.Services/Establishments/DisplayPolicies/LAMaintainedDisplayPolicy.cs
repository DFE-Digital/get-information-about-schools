using Edubase.Common;
using Edubase.Services.Enums;

namespace Edubase.Services.Establishments.DisplayPolicies
{
    public class LAMaintainedDisplayPolicy : EstablishmentDisplayPolicy
    {
        protected override bool IsMatchInternal(eLookupEstablishmentType type, eLookupEstablishmentTypeGroup typeGroup)
        {
            return typeGroup.OneOfThese(eLookupEstablishmentTypeGroup.LAMaintainedSchools, eLookupEstablishmentTypeGroup.SpecialSchools)
            &&
            type.OneOfThese(eLookupEstablishmentType.CommunitySchool, eLookupEstablishmentType.CommunitySpecialSchool,
                            eLookupEstablishmentType.FoundationSchool, eLookupEstablishmentType.FoundationSpecialSchool, eLookupEstablishmentType.LANurserySchool,
                            eLookupEstablishmentType.VoluntaryAidedSchool, eLookupEstablishmentType.VoluntaryControlledSchool);
        }

        protected override void ConfigureInternal()
        {
            HeadteacherDetails = true;
            AgeRange = true;
            GenderOfEntry = true;
            GroupDetails = true;
            LAESTAB = true;
            AdmissionsPolicy = true;
            WebsiteAddress = true;
            OfstedRatingDetails = true;
            ReligiousCharacter = true;
            Diocese = Establishment.DioceseId.HasValue;
            BoardingProvision = true;
            NurseryProvision = true;
            OfficialSixthFormProvision = true;
            Capacity = true;
            CloseDate = IsSchoolClosed;
            ReasonEstablishmentClosed = IsSchoolClosed;
            SpecialClasses = true;
            MainEmailAddress = IsUserLoggedIn;
            AlternativeEmailAddress = MainEmailAddress;
            LastChangedDate = IsUserLoggedIn;
            SENProvisions = true;
            TypeOfResourcedProvision = true;

            if (Establishment.TypeOfResourcedProvisionId
                .OneOfThese(eLookupTypeOfResourcedProvision.ResourceProvisionAndSENUnit,
                eLookupTypeOfResourcedProvision.SENUnit))
            {
                ResourcedProvisionOnRoll = true;
                ResourcedProvisionCapacity = true;
                SenUnitOnRoll = true;
                SenUnitCapacity = true;
            }
        }
    }
}