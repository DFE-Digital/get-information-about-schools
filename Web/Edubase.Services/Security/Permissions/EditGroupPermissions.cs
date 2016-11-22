using Edubase.Services.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edubase.Data.Entity.Lookups;

namespace Edubase.Services.Security.Permissions
{
    /// <summary>
    /// Whitelisting predicate; all sub-predicates must be true.
    /// </summary>
    public class EditGroupPermissions : GroupPermissions
    {
        [JsonProperty("a")]
        public bool AllGroups { get; set; }

        [JsonProperty("g")]
        public int[] GroupIds { get; set; } = new int[0];
    }

    public class CreateGroupPermissions : GroupPermissions
    {

    }

    public abstract class GroupPermissions : Permission
    {
        [JsonProperty("t")]
        public int[] TypeIds { get; set; } = new int[0];

        [JsonIgnore]
        public eLookupGroupType[] Types
        {
            get { return TypeIds.Cast<eLookupGroupType>().ToArray(); }
            set { TypeIds = value.Cast<int>().ToArray(); }
        }
    }
}
