using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Domain
{
    public class CompanySearchDto : PagedResultDto
    {
        public List<CompanyProfileDto> Items { get; set; }

        public CompanySearchDto(int skip, int take) : base(skip, take)
        {

        }
    }
}
