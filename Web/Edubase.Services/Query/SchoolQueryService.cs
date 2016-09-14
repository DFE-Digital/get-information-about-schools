using Edubase.Data.Repositories;
using Edubase.Services.Query.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Services.Api;

namespace Edubase.Services.Query
{
    public class SchoolQueryService : ISchoolQueryService
    {
        private IApiService _api;

        public SchoolQueryService(IApiService api)
        {
            _api = api;
        }

        /// <summary>
        /// Retrieves a list of schools within a given multi-academy trust
        /// </summary>
        /// <param name="matID"></param>
        /// <returns></returns>
        public dynamic GetSchoolsInMAT(short matID)
        {
            var repo = new SchoolRepository();
            var list = repo.FindByMATId(matID);
            var schools = _api.GetSchoolsByIds(list.Select(x => x.URN.ToString()), null);
            return schools;
        }
    }
}
