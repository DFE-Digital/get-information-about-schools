using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Establishments.Models;
using System.Collections.Generic;
using Edubase.Common;

namespace Edubase.Services.Establishments
{
    public interface IEstablishmentWriteService
    {
        Task SaveAsync(EstablishmentModel model, IPrincipal principal);
        Task<List<ChangeDescriptor>> GetModelChangesAsync(EstablishmentModel model);
    }
}