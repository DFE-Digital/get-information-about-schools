using Edubase.Services.Establishments.DisplayPolicies;
using Edubase.Services.Establishments.Models;

namespace Edubase.Services.Establishments.EditPolicies
{
    public class EstablishmentEditPolicyEnvelope
    {
        public EstablishmentDisplayEditPolicy EditPolicy { get; set; }
        public EstablishmentDisplayEditPolicy ApprovalsPolicy { get; set; }

        public EstablishmentEditPolicyEnvelope Initialise(EstablishmentModel establishment)
        {
            EditPolicy.Initialise(establishment);
            return this;
        }
    }
}
