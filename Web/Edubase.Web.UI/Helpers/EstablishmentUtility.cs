using System;
using System.Linq;
using Edubase.Services.Enums;

namespace Edubase.Web.UI.Helpers
{
    /// <summary>
    /// This is a utility class that helps with some general operations around establishments.
    /// </summary>
    public static class EstablishmentUtility
    {
        private static readonly int[] OfstedLinkEstablishmentTypes =
        {
            (int) eLookupEstablishmentType.Academy1619SponsorLed,
            (int) eLookupEstablishmentType.Academy1619Converter,
            (int) eLookupEstablishmentType.AcademyAlternativeProvisionConverter,
            (int) eLookupEstablishmentType.AcademyAlternativeProvisionSponsorLed,
            (int) eLookupEstablishmentType.AcademyConverter,
            (int) eLookupEstablishmentType.AcademySpecialConverter,
            (int) eLookupEstablishmentType.AcademySpecialSponsorLed,
            (int) eLookupEstablishmentType.AcademySponsorLed,
            (int) eLookupEstablishmentType.ChildrensCentre,
            (int) eLookupEstablishmentType.ChildrensCentreLinkedSite,
            (int) eLookupEstablishmentType.CityTechnologyCollege,
            (int) eLookupEstablishmentType.CommunitySchool,
            (int) eLookupEstablishmentType.CommunitySpecialSchool,
            (int) eLookupEstablishmentType.FoundationSchool,
            (int) eLookupEstablishmentType.FoundationSpecialSchool,
            (int) eLookupEstablishmentType.FreeSchools,
            (int) eLookupEstablishmentType.FreeSchools1619,
            (int) eLookupEstablishmentType.FreeSchoolsAlternativeProvision,
            (int) eLookupEstablishmentType.FreeSchoolsSpecial,
            (int) eLookupEstablishmentType.FurtherEducation,
            (int) eLookupEstablishmentType.HigherEducationInstitutions,
            (int) eLookupEstablishmentType.LANurserySchool,
            (int) eLookupEstablishmentType.NonmaintainedSpecialSchool,
            (int) eLookupEstablishmentType.OtherIndependentSchool,
            (int) eLookupEstablishmentType.OtherIndependentSpecialSchool,
            (int) eLookupEstablishmentType.PupilReferralUnit,
            (int) eLookupEstablishmentType.ServiceChildrensEducation,
            (int) eLookupEstablishmentType.SpecialPost16Institution,
            (int) eLookupEstablishmentType.StudioSchools,
            (int) eLookupEstablishmentType.UniversityTechnicalCollege,
            (int) eLookupEstablishmentType.VoluntaryAidedSchool,
            (int) eLookupEstablishmentType.VoluntaryControlledSchool
        };

        /// <summary>
        /// Checks if the supplied establishment type is applicable for Ofsted link.        
        /// </summary>
        /// <param name="establishmentTypeId"></param>
        /// <returns>true if establishment type is applicable for Ofsted link</returns>        
        public static bool IsOfstedLinkEstablishmentType(int establishmentTypeId)
        {
            return OfstedLinkEstablishmentTypes.Contains(establishmentTypeId);
        }
    }
}
