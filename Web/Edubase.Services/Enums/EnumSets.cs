using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Enums
{
    using ET = eLookupEstablishmentType;
    using GR = eLookupGovernorRole;

    public static class EnumSets
    {
        public static IEnumerable<int> LAMaintainedEstablishments { get; private set; } = new[] { ET.CommunitySchool, ET.CommunitySpecialSchool, ET.FoundationSchool, ET.FoundationSpecialSchool, ET.LANurserySchool, ET.PupilReferralUnit, ET.VoluntaryAidedSchool, ET.VoluntaryControlledSchool }.Cast<int>();
        public static IEnumerable<int> AcademiesAndFreeSchools { get; private set; } = new[] { ET.Academy1619Converter, ET.Academy1619SponsorLed, ET.AcademyAlternativeProvisionConverter, ET.AcademyAlternativeProvisionSponsorLed, ET.AcademyConverter, ET.AcademySpecialConverter, ET.AcademySpecialSponsorLed, ET.AcademySponsorLed, ET.CityTechnologyCollege, ET.FreeSchools, ET.FreeSchools1619, ET.FreeSchoolsAlternativeProvision, ET.FreeSchoolsSpecial, ET.StudioSchools, ET.UniversityTechnicalCollege }.Cast<int>();
        public static IEnumerable<int> SingularGovernorRoles { get; private set; } = new[] { GR.ChairOfGovernors, GR.ChairOfLocalGoverningBody, GR.ChairOfTrustees, GR.AccountingOfficer, GR.ChiefFinancialOfficer }.Cast<int>();
        public static IEnumerable<GR> eSingularGovernorRoles { get; private set; } = new[] { GR.ChairOfGovernors, GR.ChairOfLocalGoverningBody, GR.ChairOfTrustees, GR.AccountingOfficer, GR.ChiefFinancialOfficer };
    }
}
