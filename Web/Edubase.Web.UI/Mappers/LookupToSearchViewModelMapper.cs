using System.Collections.Generic;
using System.Linq;
using Edubase.Services.Domain;
using Edubase.Web.UI.Models;

namespace Edubase.Web.UI.Mappers.Establishment
{
    public static class LookupToSearchViewModelMapper
    {
        public static IEnumerable<HeirarchicalLookupItemViewModel> MapToHeirarchicalLookupItemViewModels(IEnumerable<EstablishmentLookupDto> establishmentTypes, IEnumerable<LookupDto> establishmentGroupTypes)
        {
            return establishmentGroupTypes.Select(groupType => MapToHeirarchicalLookupItemViewModel(establishmentTypes, groupType));
         }

        public static HeirarchicalLookupItemViewModel MapToHeirarchicalLookupItemViewModel(IEnumerable<EstablishmentLookupDto> establishmentTypes, LookupDto groupType)
        {
            return new HeirarchicalLookupItemViewModel
            {
                Id = groupType.Id,
                Name = groupType.Name,
                ChildItems = establishmentTypes.Where(c => c.GroupIds.Contains(groupType.Id))
                                .Select(e => new HeirarchicalLookupItemViewModel { Id = e.Id, Name = e.Name })
                                .ToList()
            };
        }
    }
}
