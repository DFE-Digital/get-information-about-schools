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

        public bool IsNewEntity => !Group.GroupUID.HasValue;

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
    }
}
