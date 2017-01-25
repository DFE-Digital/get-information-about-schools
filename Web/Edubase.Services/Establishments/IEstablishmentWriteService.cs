using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Establishments.Models;

namespace Edubase.Services.Establishments
{
    public interface IEstablishmentWriteService
    {
        Task SaveAsync(EstablishmentModel model, IPrincipal principal);
    }
}