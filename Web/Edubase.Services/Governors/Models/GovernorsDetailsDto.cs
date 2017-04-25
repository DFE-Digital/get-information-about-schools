using Edubase.Services.Enums;
using Edubase.Services.Governors.DisplayPolicies;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Edubase.Services.Governors.Models
{
    public class GovernorsDetailsDto
    {
        /// <summary>
        /// Which roles are applicable to the parent group or establishment
        /// </summary>
        public List<eLookupGovernorRole> ApplicableRoles { get; set; } = new List<eLookupGovernorRole>();

        /// <summary>
        /// List of current (appt. end date eq. null or in the future) governors whose roles are in the ApplicableRoles list.
        /// </summary>
        public List<GovernorModel> CurrentGovernors { get; set; }

        /// <summary>
        /// List of past (appt. end date ge. 1-year-ago and lt. today) governors whose roles are in the ApplicableRoles list.
        /// </summary>
        public List<GovernorModel> HistoricalGovernors { get; set; }

        /// <summary>
        /// Which fields are permissible for display at a field/role level.
        /// </summary>
        [JsonIgnore]
        public virtual Dictionary<eLookupGovernorRole, GovernorDisplayPolicy> RoleDisplayPolicies { get; set; } = new Dictionary<eLookupGovernorRole, GovernorDisplayPolicy>();
        
        /// <summary>
        /// Whether the user can see the maximum amount of information held about a governor.
        /// </summary>
        public bool HasFullAccess { get; set; }

        /// <summary>
        /// Returns whether there are any current or historic governors in this dto.
        /// </summary>
        public bool HasGovernors => (CurrentGovernors != null && CurrentGovernors.Any()) || (HistoricalGovernors != null && HistoricalGovernors.Any());

        public string GroupDelegationInformation { get; set; }
        public bool ShowDelegationInformation { get; set; }
    }

    public class GovernorsDetailsTexunaDto : GovernorsDetailsDto
    {
        [JsonProperty("roleDisplayPolicies")]
        public RoleDisplayPolicy[] RoleDisplayPoliciesList { get; set; }

        public override Dictionary<eLookupGovernorRole, GovernorDisplayPolicy> RoleDisplayPolicies => RoleDisplayPoliciesList.ToDictionary(x => x.GovernorRoleId, x => x.DisplayPolicy);
    }

    public class RoleDisplayPolicy
    {
        public GovernorDisplayPolicy DisplayPolicy { get; set; }
        public eLookupGovernorRole GovernorRoleId { get; set; }
    }
}
