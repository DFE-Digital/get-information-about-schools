using Edubase.Services.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.IntegrationEndPoints.CompaniesHouse.Models
{
    public class CompanySearchModel : PagedResultDto
    {
        public List<CompanyProfile> Items { get; set; }

        public CompanySearchModel(int skip, int take) : base(skip, take)
        {

        }
    }
}
