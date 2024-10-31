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

        // See also database table StaffRole, column isOnePersonRole
        public static IEnumerable<GR> eSingularGovernorRoles { get; } = new[]
        {
            GR.ChairOfGovernors,
            GR.ChairOfLocalGoverningBody,
            GR.ChairOfTrustees,
            GR.AccountingOfficer,
            GR.ChiefFinancialOfficer,
            GR.Establishment_SharedChairOfLocalGoverningBody,
            GR.GovernanceProfessionalToALocalAuthorityMaintainedSchool,
            GR.GovernanceProfessionalToAFederation,
            GR.GovernanceProfessionalToAnIndividualAcademyOrFreeSchool,
            GR.GovernanceProfessionalToAMat,
            GR.GovernanceProfessionalToASecureSat,
            GR.Establishment_SharedGovernanceProfessional,
            GR.GovernanceProfessionalToASat,
        };

        public static IEnumerable<int> SingularGovernorRoles { get; } = eSingularGovernorRoles.Cast<int>();

        // See also database table StaffRole, columns isSharing and isShared
        public static IEnumerable<GR> eSharedGovernorRoles { get; } = new[]
        {
            GR.Group_SharedChairOfLocalGoverningBody,
            GR.Group_SharedLocalGovernor,
            GR.Establishment_SharedLocalGovernor,
            GR.Establishment_SharedChairOfLocalGoverningBody,
            GR.Group_SharedGovernanceProfessional,
            GR.Establishment_SharedGovernanceProfessional
        };

        public static IEnumerable<int> SharedGovernorRoles { get; } = eSharedGovernorRoles.Cast<int>();

        // See also database table StaffRole
        public static IEnumerable<GR> eGovernanceProfessionalRoles { get; } = new[]
        {
            GR.GovernanceProfessionalToALocalAuthorityMaintainedSchool,
            GR.GovernanceProfessionalToAFederation,
            GR.GovernanceProfessionalToAnIndividualAcademyOrFreeSchool,
            GR.GovernanceProfessionalToAMat,
            GR.Group_SharedGovernanceProfessional,
            GR.Establishment_SharedGovernanceProfessional,
            GR.GovernanceProfessionalToASat,
            GR.GovernanceProfessionalToASecureSat,
        };

        public static IEnumerable<int> GovernanceProfessionalRoles = eGovernanceProfessionalRoles.Cast<int>();



        /// <summary>
        /// Every governance professional role, combined with every type of governance professional.
        /// </summary>
        public static IEnumerable<GR[]> PairwiseGovernanceProfessionalRoles =>
        (
            from preExistingRole in EnumSets.eGovernanceProfessionalRoles
            from newRole in EnumSets.eGovernanceProfessionalRoles
            select new GR[] { preExistingRole, newRole }
        );


        /// <summary>
        /// Existing role, new role
        /// </summary>
        public static IEnumerable<GR[]> PermittedGovernanceProfessionalCombinations => new List<GR[]>
        {
            // - #198772 / #193913 : MAT can have "one-each" of "Shared governance professional - group" and "Governance professional to a multi-academy trust (MAT)"
            // - #231733: Can now also have many "Shared governance professional - group" roles, not just "one of each"
            new [] {GR.Group_SharedGovernanceProfessional, GR.GovernanceProfessionalToAMat},
            new [] {GR.GovernanceProfessionalToAMat, GR.Group_SharedGovernanceProfessional},

            // - #198772 / #197361 : Establishment within a SAT can have "one-each" of "Governance professional to an individual academy or free school" and "Governance professional for single-academy trust (SAT)"
            new [] {GR.GovernanceProfessionalToAnIndividualAcademyOrFreeSchool, GR.GovernanceProfessionalToASat},
            new [] {GR.GovernanceProfessionalToASat, GR.GovernanceProfessionalToAnIndividualAcademyOrFreeSchool},

            // - #198239: System should allow adding a Governance professional to a federation if a record for Governance professional to a local authority maintained is already recorded.
            new [] {GR.GovernanceProfessionalToAFederation, GR.GovernanceProfessionalToALocalAuthorityMaintainedSchool},
            new [] {GR.GovernanceProfessionalToALocalAuthorityMaintainedSchool, GR.GovernanceProfessionalToAFederation},

            // - #231733: Can now have many "Shared governance professional - group" roles, not just "one of each"
            new [] {GR.Group_SharedGovernanceProfessional, GR.Group_SharedGovernanceProfessional},
        };

        /// All combinations of governance professional roles are forbidden, minus those which are explicitly permitted in PermittedGovernanceProfessionalCombinations
        public static IEnumerable<GR[]> ForbiddenCombinationsOfGovernanceProfessionalRoles => PairwiseGovernanceProfessionalRoles
            .Where(allPairsPair => !PermittedGovernanceProfessionalCombinations.Any(innerPair =>
                allPairsPair[0].Equals(innerPair[0])
                && allPairsPair[1].Equals(innerPair[1])));


        public static IEnumerable<GR> eChairOfLocalGoverningBodyRoles { get; } = new[]
        {
            GR.Group_SharedChairOfLocalGoverningBody,
            GR.Establishment_SharedChairOfLocalGoverningBody,
            GR.ChairOfLocalGoverningBody
        };

        public static IEnumerable<int> ChairOfLocalGoverningBodyRoles = eChairOfLocalGoverningBodyRoles.Cast<int>();
    }
}
