using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Query.Interfaces
{
    public interface ISchoolQueryService
    {
        dynamic GetSchoolsInMAT(short matID);
    }
}
