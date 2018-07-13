using Edubase.Common.Reflection;
using Edubase.Services.Enums;
using Edubase.Services.Establishments.DisplayPolicies;
using Edubase.Services.Establishments.Models;
using System.Collections.Generic;
using System.Linq;

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
