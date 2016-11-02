using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Domain
{
    public class ApprovalDto : PagedResultDto
    {   
        public int? EstablishmentUrn { get; set; }
        public string EstablishmentName { get; set; }
        public List<ApprovalItemDto> Items { get; set; }

        public ApprovalDto(int skip, int take) : base(skip, take) { }
    }
}
