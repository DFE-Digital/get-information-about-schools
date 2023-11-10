using Edubase.Services.Establishments.Models;
using Edubase.Web.UI.Areas.Governors.Models;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Edubase.Web.UI.Areas
{
    public interface IGovernorsGridViewModelFactory
    {
        Task<GovernorsGridViewModel> CreateGovernorsViewModel(int? groupUId = null,
            int? establishmentUrn = null, EstablishmentModel establishmentModel = null, IPrincipal user = null);
    }
}
