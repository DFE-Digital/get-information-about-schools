using System;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Groups.Models;

namespace Edubase.Web.UI.Helpers
{
    public interface ILayoutHelper
    {
        Task PopulateLayoutProperties(object viewModel, int? establishmentUrn, int? groupUId, IPrincipal user, Action<EstablishmentModel> processEstablishment = null, Action<GroupModel> processGroup = null);
    }
}