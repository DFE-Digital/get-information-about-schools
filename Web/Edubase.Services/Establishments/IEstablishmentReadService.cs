using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using Edubase.Services.Domain;
using Edubase.Services.Establishments.DisplayPolicies;
using Edubase.Services.Establishments.Models;
using Edubase.Services.Groups.Models;
using Edubase.Services.Establishments.Search;

namespace Edubase.Services.Establishments
{
    public interface IEstablishmentReadService
    {
        Task<EstablishmentModel> GetAsync(int urn, IPrincipal principal);
        Task<IEnumerable<EstablishmentChangeDto>> GetChangeHistoryAsync(int urn, int take, IPrincipal user);
        EstablishmentDisplayPolicy GetDisplayPolicy(IPrincipal user, EstablishmentModel establishment, GroupModel group);
        Task<IEnumerable<LinkedEstablishmentModel>> GetLinkedEstablishments(int urn);
        Task<IEnumerable<ChangeDescriptorDto>> GetPendingChangesAsync(int urn, IPrincipal principal);
        Task<IEnumerable<EstablishmentSuggestionItem>> SuggestAsync(string text, int take = 10);
    }
}