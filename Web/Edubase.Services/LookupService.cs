using Edubase.Data.Entity;
using Edubase.Data.Entity.Lookups;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services
{
    public class LookupService
    {
        private Dictionary<string, Func<ApplicationDbContext, int, string>> _mapping = new Dictionary<string, Func<ApplicationDbContext, int, string>>()
        {
            { "LocalAuthorityId", (dc, id) => dc.LocalAuthorities.FirstOrDefault(x=>x.Id==id)?.Name },
            { "HeadTitleId", (dc, id) => dc.HeadTitles.FirstOrDefault(x=>x.Id==id)?.Name },
            { "GenderId", (dc, id) => dc.Genders.FirstOrDefault(x=>x.Id==id)?.Name },
            { "EducationPhaseId", (dc, id) => dc.EducationPhases.FirstOrDefault(x=>x.Id==id)?.Name },
            { "AdmissionsPolicyId", (dc, id) => dc.AdmissionsPolicies.FirstOrDefault(x=>x.Id==id)?.Name },
            { "StatusId", (dc, id) => dc.EstablishmentStatuses.FirstOrDefault(x=>x.Id==id)?.Name }
        };
        
        public LookupService()
        {

        }

        public string GetName(string lookupName, int id)
        {
            if (IsLookupField(lookupName))
            {
                using (var dc = new ApplicationDbContext())
                    return _mapping[lookupName]?.Invoke(dc, id);
            }
            else throw new Exception($"The lookup '{lookupName}' is not a valid lookup item");
        }

        public bool IsLookupField(string name) => _mapping.ContainsKey(name);
    }
}
