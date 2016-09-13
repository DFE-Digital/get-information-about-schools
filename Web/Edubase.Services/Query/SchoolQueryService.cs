using Edubase.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Query
{
    public class SchoolQueryService
    {
        /// <summary>
        /// Retrieves a list of schools within a given multi-academy trust
        /// </summary>
        /// <param name="matID"></param>
        /// <returns></returns>
        public dynamic GetSchoolsInMAT(short matID)
        {
            var repo = new SchoolRepository();
            var list = repo.FindByMATId(matID);

            // go through list and get rest of the data from the SPT API

            return null;
        }
    }
}
