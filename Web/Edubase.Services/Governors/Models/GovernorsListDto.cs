using Edubase.Services.Enums;
using Edubase.Services.Governors.DisplayPolicies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Governors.Models
{
    public class GovernorsListDto
    {
        public GovernorDisplayPolicy DisplayPolicy { get; set; }
        public List<eLookupGovernorRole> ApplicableRoles { get; set; }
        public IEnumerable<GovernorModel> CurrentGovernors { get; set; }
        public IEnumerable<GovernorModel> HistoricGovernors { get; set; }
    }
}
