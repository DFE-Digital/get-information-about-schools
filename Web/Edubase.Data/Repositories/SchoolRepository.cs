using Edubase.Data.Entity;
using Edubase.Data.Stubs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Edubase.Data.Repositories
{
    public class SchoolRepository
    {
        public IEnumerable<SchoolMAT> FindByMATId(short id, int skip = 0, int take = 50)
        {
            return new[]
            {
                new SchoolMAT() {  URN=100053, LinkedUID=232 }
            };
        }
    }
}
