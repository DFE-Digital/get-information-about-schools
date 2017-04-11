using Edubase.Services.Governors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edubase.Services.Governors.Models;
using System.Security.Principal;

namespace Edubase.Services.Texuna.Governors
{
    public class GovernorsWriteApiService : IGovernorsWriteService
    {
        public Task DeleteAsync(int id, IPrincipal principal)
        {
            throw new NotImplementedException();
        }

        public Task<int> SaveAsync(GovernorModelBase model, IPrincipal principal)
        {
            throw new NotImplementedException();
        }
    }
}
