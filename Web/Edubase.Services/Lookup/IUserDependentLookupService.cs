using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Edubase.Services.Domain;

namespace Edubase.Services.Lookup
{
    public interface IUserDependentLookupService
    {
        Task<IEnumerable<LookupDto>> EstablishmentStatusesGetAllAsync(IPrincipal user);
    }
}
