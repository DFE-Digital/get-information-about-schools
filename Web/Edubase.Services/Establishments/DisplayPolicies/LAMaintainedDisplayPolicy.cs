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
            GenderId = true;
            GroupDetails = true;
            EstablishmentNumber = true;
            AdmissionsPolicyId = true;
            Contact_WebsiteAddress = true;
            OfstedRatingDetails = true;
            ReligiousCharacterId = true;
            DioceseId = Establishment.DioceseId.HasValue;
            ProvisionBoardingId = true;
            ProvisionNurseryId = true;
            ProvisionOfficialSixthFormId = true;
            Capacity = true;
            CloseDate = IsSchoolClosed;
            ReasonEstablishmentClosedId = IsSchoolClosed;
            ProvisionSpecialClassesId = true;
            Contact_EmailAddress = IsUserLoggedIn;
            ContactAlt_EmailAddress = IsUserLoggedIn;
            LastChangedDate = IsUserLoggedIn;
            TypeOfSENProvisionList = true;
            TypeOfResourcedProvisionId = true;

            if (Establishment.TypeOfResourcedProvisionId
                .OneOfThese(eLookupTypeOfResourcedProvision.ResourceProvisionAndSENUnit,
                eLookupTypeOfResourcedProvision.ResourceProvision))
            {
                ResourcedProvisionOnRoll = true;
                ResourcedProvisionCapacity = true;
                SenUnitOnRoll = true;
                SenUnitCapacity = true;
            }
        }
    }
}