using Edubase.Services.Governors.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Governors
{
    public interface IGovernorsWriteService
    {
        Task<int> SaveAsync(GovernorModelBase model, IPrincipal principal);
        Task DeleteAsync(int id, IPrincipal principal);
    }
}
