using Newtonsoft.Json;
using System.Collections.Generic;

namespace Edubase.Services.Groups.Models
{
    public class SaveGroupDto
    {
        public GroupModel Group { get; set; }
        public List<LinkedEstablishmentGroup> LinkedEstablishments { get; set; }

        [JsonIgnore]
        public bool IsNewEntity => !GetGroupUId().HasValue;

        private int? _groupUId = null;

        public int? GetGroupUId() => _groupUId ?? Group?.GroupUId;

        public int? GroupUId
        {
            get
            {
                return GetGroupUId();
            }
            set
            {
                _groupUId = value;
                if (Group != null) Group.GroupUId = value;
            }
        }

        public SaveGroupDto()
        {

        }

        public SaveGroupDto(GroupModel group) 
            : this(group, null)
        {
        }

        public SaveGroupDto(GroupModel group, List<LinkedEstablishmentGroup> linkedEstablishments)
        {
            Group = group;
            LinkedEstablishments = linkedEstablishments;
        }

        public SaveGroupDto(int groupUId, List<LinkedEstablishmentGroup> linkedEstablishments)
        {
            _groupUId = groupUId;
            LinkedEstablishments = linkedEstablishments;
        }

    }
}
