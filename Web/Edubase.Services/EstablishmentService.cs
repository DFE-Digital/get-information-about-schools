using Edubase.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services
{
    public class EstablishmentService
    {
        public string GetName(int urn)
        {
            using (var dc = new ApplicationDbContext())
                return dc.Establishments.FirstOrDefault(x => x.Urn == urn)?.Name;
        }
    }
}
