using Edubase.Services.Enums;
using Edubase.Web.UI.Helpers;

namespace Edubase.Web.UI.Models.DisplayProfiles
{
    public class LAMaintainedDisplayProfile : EstablishmentDisplayProfile
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
            var isUserLoggedIn = Principal.Identity.IsAuthenticated;
            var isSchoolClosed = Establishment.StatusId == (int)eLookupEstablishmentStatus.Closed;

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
            CloseDate = isSchoolClosed;
            ReasonEstablishmentClosed = isSchoolClosed;
            SpecialClasses = true;
            MainEmailAddress = isUserLoggedIn;
            AlternativeEmailAddress = MainEmailAddress;
            LastChangedDate = isUserLoggedIn;
            SENProvisions = true;
            TypeOfResourcedProvision = true;
            ResourcedProvisionOnRoll = true;
            ResourcedProvisionCapacity = true;
            SenUnitOnRoll = true;
            SenUnitCapacity = true;
        }
    }
}