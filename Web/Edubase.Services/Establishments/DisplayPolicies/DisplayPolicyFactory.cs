using Edubase.Data.Entity;
using Edubase.Services;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Groups.Models;
using System;
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
            new ChildrensCentresDisplayPolicy(),
            new ChildrensCentreLinkedSitesDisplayPolicy()
        };

        public EstablishmentDisplayPolicy Create(IPrincipal principal, EstablishmentModelBase establishment, GroupModel group) 
            => _profiles.SingleOrThrow(x => x.IsMatch(establishment), 
                () => new Exception($"A Display Profile could not be found for Establishment (TypeId: {establishment.TypeId}, EstablishmentTypeGroupId: {establishment.EstablishmentTypeGroupId}. (FYI, An Establishment needs both a Type and TypeGroup; if they're populated, then no Display Profile is available for the Type/TypeGroup combination.)")).Configure(principal, establishment, group);
    }
}