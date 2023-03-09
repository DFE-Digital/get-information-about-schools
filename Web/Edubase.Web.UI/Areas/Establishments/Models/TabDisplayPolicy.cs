using System.Linq;
using System.Security.Principal;
using Edubase.Common;
using Edubase.Services.Enums;
using Edubase.Services.Establishments.DisplayPolicies;
using Edubase.Services.Establishments.Models;

namespace Edubase.Web.UI.Areas.Establishments.Models
{
    using ET = eLookupEstablishmentType;

    public class TabDisplayPolicy
    {
        public bool Details { get; set; } = true;
        public bool IEBT { get; set; }
        public bool Governance { get; set; }
        public bool Links { get; set; } = true;
        public bool Location { get; set; } = true;
        public bool ChangeHistory { get; set; } = true;

        public bool Helpdesk { get; set; }

        public TabDisplayPolicy(EstablishmentModel model, EstablishmentDisplayEditPolicy policy, IPrincipal principal)
        {
            IEBT = policy.IEBTDetail.Any();

            Helpdesk = policy.HelpdeskNotes;

            Governance = model.TypeId.OneOfThese(
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
                    ET.CommunitySchool,
                    ET.CommunitySpecialSchool,
                    ET.FoundationSchool,
                    ET.FoundationSpecialSchool,
                    ET.LANurserySchool,
                    ET.PupilReferralUnit,
                    ET.VoluntaryAidedSchool,
                    ET.VoluntaryControlledSchool);

            Location = new[]
            {
                policy.RSCRegionId, policy.GovernmentOfficeRegionId, policy.AdministrativeDistrictId, policy.AdministrativeWardId, policy.ParliamentaryConstituencyId,
                policy.UrbanRuralId, policy.GSSLAId, policy.Easting, policy.Northing, policy.MSOAId, policy.LSOAId
            }.Any(x => x == true);

            Links = !model.TypeId.OneOfThese(ET.SecureAcademies16to19);
        }

        public TabDisplayPolicy()
        {

        }
    }
}
