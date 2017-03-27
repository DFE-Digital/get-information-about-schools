using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Groups.Models
{
    public class SaveGroupDto
    {
        public GroupModel Group { get; set; }
        public List<EstablishmentGroupModel> LinkedEstablishments { get; set; }

        public bool IsNewEntity => !GetGroupUId().HasValue;

        private int? _groupUId = null;

        public int? GetGroupUId() => _groupUId ?? Group?.GroupUID;

        public SaveGroupDto()
        {

        }

        public SaveGroupDto(GroupModel group) 
            : this(group, null)
        {
        }

        public SaveGroupDto(GroupModel group, List<EstablishmentGroupModel> linkedEstablishments)
        {
            Group = group;
            LinkedEstablishments = linkedEstablishments;
        }

        public SaveGroupDto(int groupUId, List<EstablishmentGroupModel> linkedEstablishments)
        {
            _groupUId = groupUId;
            LinkedEstablishments = linkedEstablishments;
        }

    }
}
