using Edubase.Services.Enums;
using Edubase.Web.UI.Helpers;

namespace Edubase.Web.UI.Models.DisplayProfiles
{
    public class AcademyDisplayProfile : EstablishmentDisplayProfile
    {
        protected override bool IsMatchInternal(eLookupEstablishmentType type, eLookupEstablishmentTypeGroup typeGroup)
        {
            return typeGroup.OneOfThese(eLookupEstablishmentTypeGroup.Academies, 
                                        eLookupEstablishmentTypeGroup.FreeSchools, 
                                        eLookupEstablishmentTypeGroup.IndependentSchools)
            &&
            type.OneOfThese(eLookupEstablishmentType.Academy1619Converter,
                            eLookupEstablishmentType.Academy1619SponsorLed,
                            eLookupEstablishmentType.AcademyAlternativeProvisionConverter,
                            eLookupEstablishmentType.AcademyAlternativeProvisionSponsorLed,
                            eLookupEstablishmentType.AcademyConverter,
                            eLookupEstablishmentType.AcademySpecialConverter,
                            eLookupEstablishmentType.AcademySpecialSponsorLed,
                            eLookupEstablishmentType.AcademySponsorLed,
                            eLookupEstablishmentType.FreeSchools,
                            eLookupEstablishmentType.FreeSchools1619,
                            eLookupEstablishmentType.FreeSchoolsAlternativeProvision,
                            eLookupEstablishmentType.FreeSchoolsSpecial,
                            eLookupEstablishmentType.StudioSchools,
                            eLookupEstablishmentType.UniversityTechnicalCollege,
                            eLookupEstablishmentType.CityTechnologyCollege);
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
            ReligiousEhtos = true;
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