using Edubase.Common;
using Edubase.Services.Enums;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Security;
using System.Linq;
using System.Security.Principal;

namespace Edubase.Web.UI.Models.Establishments
{
    using Services.Establishments.DisplayPolicies;
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
                    ET.FurtherEducation,
                    ET.SixthFormCentres,
                    ET.CityTechnologyCollege,
                    ET.FreeSchools,
                    ET.FreeSchools1619,
                    ET.FreeSchoolsAlternativeProvision,
                    ET.FreeSchoolsSpecial,
                    ET.StudioSchools,
                    ET.UniversityTechnicalCollege,
                    ET.CommunitySchool,
                    ET.FoundationSchool,
                    ET.LANurserySchool,
                    ET.PupilReferralUnit,
                    ET.VoluntaryAidedSchool,
                    ET.VoluntaryControlledSchool);

            Location = new[]
            {
                policy.RSCRegionId, policy.GovernmentOfficeRegionId, policy.AdministrativeDistrictId, policy.AdministrativeWardId, policy.ParliamentaryConstituencyId,
                policy.UrbanRuralId, policy.GSSLAId, policy.Easting, policy.Northing, policy.CASWardId, policy.MSOAId, policy.LSOAId
            }.Any(x => x == true);
        }

        public TabDisplayPolicy()
        {

        }
    }
}