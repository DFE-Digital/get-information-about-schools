using System.Collections.Generic;
using System.Linq;

namespace Edubase.Services.Enums
{
    using ET = eLookupEstablishmentType;
    using GR = eLookupGovernorRole;

    public static class EnumSets
    {
        public static IEnumerable<int> LAMaintainedEstablishments { get; } = new[]
        {
            ET.CommunitySchool,
            ET.CommunitySpecialSchool,
            ET.FoundationSchool,
            ET.FoundationSpecialSchool,
            ET.LANurserySchool,
            ET.PupilReferralUnit,
            ET.VoluntaryAidedSchool,
            ET.VoluntaryControlledSchool
        }.Cast<int>();

        public static IEnumerable<int> AcademiesAndFreeSchools { get; } = new[]
        {
            ET.Academy1619Converter,
            ET.Academy1619SponsorLed,
            ET.AcademyAlternativeProvisionConverter,
            ET.AcademyAlternativeProvisionSponsorLed,
            ET.AcademyConverter,
            ET.AcademySpecialConverter,
            ET.AcademySpecialSponsorLed,
            ET.AcademySponsorLed,
            ET.CityTechnologyCollege,
            ET.FreeSchools,
            ET.FreeSchools1619,
            ET.FreeSchoolsAlternativeProvision,
            ET.FreeSchoolsSpecial,
            ET.StudioSchools,
            ET.UniversityTechnicalCollege,
            ET.AcademySecure16to19
        }.Cast<int>();

        public static IEnumerable<GR> eSingularGovernorRoles { get; } = new[]
        {
            GR.ChairOfGovernors,
            GR.ChairOfLocalGoverningBody,
            GR.ChairOfTrustees,
            GR.AccountingOfficer,
            GR.ChiefFinancialOfficer,
            GR.Establishment_SharedChairOfLocalGoverningBody
        };

        public static IEnumerable<int> SingularGovernorRoles { get; } = eSingularGovernorRoles.Cast<int>();

        public static IEnumerable<GR> eSharedGovernorRoles { get; } = new[]
        {
            GR.Group_SharedChairOfLocalGoverningBody,
            GR.Group_SharedLocalGovernor,
            GR.Establishment_SharedLocalGovernor,
            GR.Establishment_SharedChairOfLocalGoverningBody
        };

        public static IEnumerable<int> SharedGovernorRoles { get; } = eSharedGovernorRoles.Cast<int>();
    }
}
