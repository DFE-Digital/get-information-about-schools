#if(!TEXAPI)
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
            GenderId = true;
            GroupDetails = true;
            EstablishmentNumber = true;
            AdmissionsPolicyId = true;
            Contact_WebsiteAddress = true;
            OfstedRatingDetails = true;
            ReligiousCharacterId = true;
            DioceseId = Establishment.DioceseId.HasValue;
            ReligiousEthosId = true;
            ProvisionBoardingId = true;
            ProvisionNurseryId = true;
            ProvisionOfficialSixthFormId = true;
            Capacity = true;
            CloseDate = IsSchoolClosed;
            ReasonEstablishmentClosedId = IsSchoolClosed;
            ProvisionSpecialClassesId = true;
            Contact_EmailAddress = ContactAlt_EmailAddress = IsUserLoggedIn;
            LastChangedDate = IsUserLoggedIn;
            TypeOfSENProvisionList = true;
            TypeOfResourcedProvisionId = true;

            if(Establishment.TypeOfResourcedProvisionId
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
#endif