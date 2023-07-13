using System.Linq;
using System.Security.Principal;
using Edubase.Common;
using Edubase.Services.Enums;
using Edubase.Services.Establishments.DisplayPolicies;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Security;

namespace Edubase.Web.UI.Areas.Establishments.Models
{
    using ES = eLookupEstablishmentStatus;

    public class TabEditPolicy
    {
        public bool Details { get; set; } = true;
        public bool IEBT { get; set; } = true;
        public bool Governance { get; set; } = true;
        public bool Links { get; set; } = true;
        public bool Location { get; set; } = true;
        public bool ChangeHistory { get; set; } = true;
        public bool Helpdesk { get; set; } = true;

        public TabEditPolicy(EstablishmentModel model, EstablishmentDisplayEditPolicy policy, IPrincipal principal)
        {
            Governance = !(principal.IsInRole(EdubaseRoles.ESTABLISHMENT) && model.StatusId.OneOfThese(ES.Closed));
            Location = new[]
            {
                // these fields appear on the Locations tab
                policy.RSCRegionId, policy.GovernmentOfficeRegionId, policy.AdministrativeDistrictId, policy.AdministrativeWardId, policy.ParliamentaryConstituencyId,
                policy.UrbanRuralId, policy.GSSLAId, policy.Easting, policy.Northing, policy.MSOAId, policy.LSOAId
            }.Any(x => x == true);
        }

        public TabEditPolicy()
        {

        }
    }
}
