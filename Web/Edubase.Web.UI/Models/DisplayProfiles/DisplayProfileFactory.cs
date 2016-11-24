using Edubase.Data.Entity;
using System.Linq;
using System.Security.Principal;

namespace Edubase.Web.UI.Models.DisplayProfiles
{
    public class DisplayProfileFactory
    {
        private EstablishmentDisplayProfile[] _profiles = new EstablishmentDisplayProfile[]
        {
            new AcademyDisplayProfile(),
            new LAMaintainedDisplayProfile(),
            new IndependentNMSSDisplayProfile(),
            new BSODisplayProfile(),
            new FEHEDisplayProfile(),
            new PRUDisplayProfile(),
            new WelshDisplayProfile(),
            new SpecialPost16InstitutionDisplayProfile(),
            new OtherDisplayProfile(),
            new ChildrensCentresDisplayProfile()
        };

        public EstablishmentDisplayProfile Get(IPrincipal principal, Establishment establishment, Trust group) 
            => _profiles.Single(x => x.IsMatch(establishment)).Configure(principal, establishment, group);
    }
}