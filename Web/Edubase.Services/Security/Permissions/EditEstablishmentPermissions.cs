using Edubase.Services.Enums;
using Edubase.Services.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Security.Permissions
{
    /// <summary>
    /// Whitelisting predicate; all sub-predicates must be true.
    /// </summary>
    public class EditEstablishmentPermissions : EstablishmentPermissions
    {
        [JsonProperty("a")]
        public bool AllUrns { get; set; }

        [JsonProperty("u")]
        public int[] Urns { get; set; } = new int[0];
        
        public bool IsUrnAllowed(int urn) => AllUrns || Urns.Contains(urn);

        [JsonIgnore]
        public bool HasNoEditingPermission => !AllUrns
            && !LocalAuthorityIds.Any()
            && !GroupIds.Any()
            && !EstablishmentTypeIds.Any();

        public bool CanEdit(int urn, int? typeId, int? groupId, int? localAuthorityId, int? typeGroupId)
            => IsUrnAllowed(urn)
            && IsGroupAllowed(groupId)
            && IsLAAllowed(localAuthorityId)
            && IsTypeAllowed(typeId)
            && IsTypeGroupAllowed(typeGroupId); //todo: need to support anding and oring!!!!
    }

    public class CreateEstablishmentPermissions : EstablishmentPermissions
    {
    }

    public abstract class EstablishmentPermissions : Permission
    {
        [JsonProperty("l")]
        public int[] LocalAuthorityIds { get; set; } = new int[0];

        [JsonProperty("t")]
        public int[] EstablishmentTypeIds { get; set; } = new int[0];

        [JsonProperty("tg")]
        public int[] EstablishmentTypeGroupIds { get; set; } = new int[0];


        [JsonIgnore]
        public eLocalAuthority[] LocalAuthorities
        {
            get { return LocalAuthorityIds.Cast<eLocalAuthority>().ToArray(); }
            set { LocalAuthorityIds = value.Cast<int>().ToArray(); }
        }

        [JsonIgnore]
        public eLookupEstablishmentType[] EstablishmentTypes {
            get { return EstablishmentTypeIds.Cast<eLookupEstablishmentType>().ToArray(); }
            set { EstablishmentTypeIds = value.Cast<int>().ToArray(); }
        }

        [JsonIgnore]
        public eLookupEstablishmentTypeGroup[] EstablishmentTypeGroups
        {
            get { return EstablishmentTypeGroupIds.Cast<eLookupEstablishmentTypeGroup>().ToArray(); }
            set { EstablishmentTypeGroupIds = value.Cast<int>().ToArray(); }
        }

        [JsonProperty("g")]
        public int[] GroupIds { get; set; } = new int[0];

        public bool IsTypeAllowed(int? typeId)
            => !EstablishmentTypeIds.Any() || (typeId.HasValue && EstablishmentTypeIds.Contains(typeId.Value));
        public bool IsTypeGroupAllowed(int? typeGroupId)
                    => !EstablishmentTypeGroupIds.Any() || (typeGroupId.HasValue && EstablishmentTypeGroupIds.Contains(typeGroupId.Value));

        public bool IsLAAllowed(int? localAuthorityId)
            => !LocalAuthorityIds.Any() || (localAuthorityId.HasValue && LocalAuthorityIds.Contains(localAuthorityId.Value));

        public bool IsGroupAllowed(int? groupId)
            => !GroupIds.Any() || (groupId.HasValue && GroupIds.Contains(groupId.Value));

        public bool IsEstabTypeRestricted() => EstablishmentTypeIds.Any();
        public bool IsLARestricted() => LocalAuthorityIds.Any();
        public bool IsGroupRestricted() => GroupIds.Any();

        
    }
}
