using Edubase.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Domain
{
    public class ChangeDescriptorDto
    {
        public string PropertyName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }

        public ChangeDescriptorDto()
        {

        }

        public ChangeDescriptorDto(EstablishmentApprovalQueue entity)
        {
            PropertyName = entity.Name;
            OldValue = entity.OldValue;
            NewValue = entity.NewValue;
        }
    }
}
