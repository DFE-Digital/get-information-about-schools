using Edubase.Data.Entity;
using System.Collections.Generic;
using System.Linq;

namespace Edubase.Data.Repositories
{
    public class SchoolRepository
    {
        public IEnumerable<SchoolMAT> FindByMATId(short id, int skip = 0, int take = 50)
        {
            using (var dc = new ApplicationDbContext())
                return dc.SchoolMATs.Where(x => x.LinkedUID.Equals(id)).OrderBy(x => x.URN).Skip(skip).Take(take).ToList();
        }
    }
}
