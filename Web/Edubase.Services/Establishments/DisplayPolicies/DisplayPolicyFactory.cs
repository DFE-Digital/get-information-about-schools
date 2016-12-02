using Edubase.Data.Entity;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Groups.Models;
using System.Linq;
using System.Security.Principal;

namespace Edubase.Services.Establishments.DisplayPolicies
{
    public class DisplayPolicyFactory
    {
        private EstablishmentDisplayPolicy[] _profiles = new EstablishmentDisplayPolicy[]
        {
            new AcademyDisplayPolicy(),
            new LAMaintainedDisplayPolicy(),
            new IndependentNMSSDisplayPolicy(),
            new BSODisplayPolicy(),
            new FEHEDisplayPolicy(),
            new PRUDisplayPolicy(),
            new WelshDisplayPolicy(),
            new SpecialPost16InstitutionDisplayPolicy(),
            new OtherDisplayPolicy(),
            new ChildrensCentresDisplayPolicy()
        };

        public EstablishmentDisplayPolicy Create(IPrincipal principal, EstablishmentModel establishment, GroupModel group) 
            => _profiles.Single(x => x.IsMatch(establishment)).Configure(principal, establishment, group);
    }
}