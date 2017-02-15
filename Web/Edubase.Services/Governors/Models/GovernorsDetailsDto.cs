using Edubase.Services.Enums;
using Edubase.Services.Governors.DisplayPolicies;
using System.Collections.Generic;
using System.Linq;

namespace Edubase.Services.Governors.Models
{
    public class GovernorsDetailsDto
    {
        /// <summary>
        /// Which roles are applicable to the parent group or establishment
        /// </summary>
        public List<eLookupGovernorRole> ApplicableRoles { get; internal set; } = new List<eLookupGovernorRole>();

        /// <summary>
        /// List of current (appt. end date eq. null or in the future) governors whose roles are in the ApplicableRoles list.
        /// </summary>
        public IEnumerable<GovernorModel> CurrentGovernors { get; internal set; }

        /// <summary>
        /// List of past (appt. end date ge. 1-year-ago and lt. today) governors whose roles are in the ApplicableRoles list.
        /// </summary>
        public IEnumerable<GovernorModel> HistoricGovernors { get; internal set; }

        /// <summary>
        /// Which fields are permissible for display at a field/role level.
        /// </summary>
        public Dictionary<eLookupGovernorRole, GovernorDisplayPolicy> RoleDisplayPolicies { get; set; } = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>();
        
        /// <summary>
        /// Whether the user can see the maximum amount of information held about a governor.
        /// </summary>
        public bool HasFullAccess { get; internal set; }

        /// <summary>
        /// Returns whether there are any current or historic governors in this dto.
        /// </summary>
        public bool HasGovernors => (CurrentGovernors != null && CurrentGovernors.Any()) || (HistoricGovernors != null && HistoricGovernors.Any());
    }
}
