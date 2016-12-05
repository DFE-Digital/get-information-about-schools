using Edubase.Services.Groups.Models;
using Edubase.Services.Groups.Search;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Edubase.Services.Groups
{
    public interface IGroupReadService
    {
        Task<GroupModel> GetByEstablishmentUrnAsync(int urn);

        Task<IEnumerable<GroupSuggestionItem>> SuggestAsync(string text, int take = 10);
    }
}