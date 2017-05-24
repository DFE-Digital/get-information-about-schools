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
    using TG = eLookupEstablishmentTypeGroup;

    public class TabDisplayPolicy
    {
        public bool Details { get; set; } = true;
        public bool IEBT { get; set; }
        public bool Governance { get; set; }
        public bool Links { get; set; } = true;
        public bool Location { get; set; } = true;
        public bool ChangeHistory { get; set; } = true;

        public TabDisplayPolicy(EstablishmentModel model, EstablishmentDisplayEditPolicy displayPolicy, IPrincipal principal)
        {
            IEBT = displayPolicy.IEBTDetail.Any(); // TODO: TEXCHANGE

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

            Location = !model.TypeId.OneOfThese(ET.BritishSchoolsOverseas);
        }
    }
}