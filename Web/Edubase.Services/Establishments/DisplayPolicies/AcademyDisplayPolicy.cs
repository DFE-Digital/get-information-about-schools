using Edubase.Common;
using Edubase.Services.Enums;

namespace Edubase.Services.Establishments.DisplayPolicies
{
    public class AcademyDisplayPolicy : EstablishmentDisplayPolicy
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
            CloseDate = IsSchoolClosed;
            ReasonEstablishmentClosed = IsSchoolClosed;
            SpecialClasses = true;
            MainEmailAddress = IsUserLoggedIn;
            AlternativeEmailAddress = MainEmailAddress;
            LastChangedDate = IsUserLoggedIn;
            SENProvisions = true;
            TypeOfResourcedProvision = true;

            if(Establishment.TypeOfResourcedProvisionId
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