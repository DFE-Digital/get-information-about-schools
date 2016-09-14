using Edubase.Data.Entity;
using System.Collections.Generic;
using System.Linq;

namespace Edubase.Data.Repositories
{
    public class MATRepository
    {
        public IList<MAT> Search(string text, int skip = 0, int take = 10) => Search(text, text, skip, take);

        public IList<MAT> Search(string name, string companiesHouseNumber, int skip, int take)
        {
            using (var dc = new ApplicationDbContext())
                return dc.MATs.Where(x => x.GroupName.Contains(name) || x.CompaniesHouseNumber.Equals(companiesHouseNumber))
                    .OrderBy(x => x.GroupName).Skip(skip).Take(take).ToList();
        }

        public MAT Find(short groupUID)
        {
            using (var dc = new ApplicationDbContext())
                return dc.MATs.FirstOrDefault(x => x.GroupUID == groupUID);
        }
    }
}
