using Edubase.Services.Domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edubase.Services.Enums;

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

        public virtual bool CanEdit(int id, int? typeId, int? localAuthorityId) => IsIdAllowed(id) && IsTypeAllowed(typeId) && IsLAAllowed(localAuthorityId);

        public bool IsIdAllowed(int id) => AllGroups || GroupIds.Contains(id);
    }

    public class CreateGroupPermissions : GroupPermissions
    {
        public virtual bool CanCreate(int? typeId, int? localAuthorityId) => IsTypeAllowed(typeId) && IsLAAllowed(localAuthorityId);
    }

    public class NoCreateGroupPermissions : CreateGroupPermissions
    {
        public override bool CanCreate(int? typeId, int? localAuthorityId) => false;
        public override bool IsLAAllowed(int? localAuthorityId) => false;
        public override bool IsTypeAllowed(int? typeId) => false;
    }

    public class NoEditGroupPermissions : EditGroupPermissions
    {
        public override bool CanEdit(int id, int? typeId, int? localAuthorityId) => false;
        public override bool IsLAAllowed(int? localAuthorityId) => false;
        public override bool IsTypeAllowed(int? typeId) => false;
    }

    public abstract class GroupPermissions : Permission
    {
        [JsonProperty("l")]
        public int[] LocalAuthorityIds { get; set; } = new int[0];

        [JsonIgnore]
        public eLocalAuthority[] LocalAuthorities
        {
            get { return LocalAuthorityIds.Cast<eLocalAuthority>().ToArray(); }
            set { LocalAuthorityIds = value.Cast<int>().ToArray(); }
        }

        public virtual bool IsLAAllowed(int? localAuthorityId)
            => !LocalAuthorityIds.Any() || (localAuthorityId.HasValue && LocalAuthorityIds.Contains(localAuthorityId.Value));

        [JsonProperty("t")]
        public int[] TypeIds { get; set; } = new int[0];

        [JsonIgnore]
        public eLookupGroupType[] Types
        {
            get { return TypeIds.Cast<eLookupGroupType>().ToArray(); }
            set { TypeIds = value.Cast<int>().ToArray(); }
        }

        public virtual bool IsTypeAllowed(int? typeId) => !TypeIds.Any() || (typeId.HasValue && TypeIds.Contains(typeId.Value));
    }
}
