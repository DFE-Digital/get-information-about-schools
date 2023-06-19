using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edubase.Services.Domain;
using Edubase.Services.Lookup;
using Edubase.Web.UI.Mappers.Establishment;
using Moq;
using Xunit;

namespace Edubase.Web.UIUnitTests.Mappers
{
    public class LookupToSearchViewModelMapperTests
    {
        [Fact]
        public void MapEstablishmentTypesToSingleGroupType()
        {
            var establishmentTypes = new List<EstablishmentLookupDto> {
                new EstablishmentLookupDto() { GroupIds = new List<int> {10}, Id = 0, Name = "Establishment in Group 10"},
                new EstablishmentLookupDto() { GroupIds = new List<int> {9}, Id = 1, Name = "Establishment in Group 9"},
                new EstablishmentLookupDto() { GroupIds = new List<int> {10}, Id = 2, Name = "Establishment in Group 10"}
            };
            var groupType = new LookupDto() { Id = 10, Name = "Group 10" };

            var result = LookupToSearchViewModelMapper.MapToHeirarchicalLookupItemViewModel(establishmentTypes, groupType);

            var childItems = result.ChildItems.ToArray();
            Assert.Equal(2, childItems.Length);
            Assert.Equal("Establishment in Group 10", childItems[0].Name);
            Assert.Equal("Establishment in Group 10", childItems[1].Name);
        }

        [Fact]
        public void MapEstablishmentTypesToGroupTypes()
        {
            var establishmentTypes = new List<EstablishmentLookupDto> {
                new EstablishmentLookupDto() { GroupIds = new List<int> {9, 10}, Id = 0, Name = "Establishment in Group 9 and 10"},
                new EstablishmentLookupDto() { GroupIds = new List<int> {9, 11}, Id = 1, Name = "Establishment in Group 9"},
                new EstablishmentLookupDto() { GroupIds = new List<int> {10}, Id = 2, Name = "Establishment in Group 10"}
            };
            var groupTypes = new List<EstablishmentLookupDto>() {
                new EstablishmentLookupDto() { Id = 10, Name = "Group 10" },
                new EstablishmentLookupDto() { Id = 9, Name = "Group 9" },
                new EstablishmentLookupDto() { Id = 99, Name = "Group 99" }
            };
            var result = LookupToSearchViewModelMapper.MapToHeirarchicalLookupItemViewModels(establishmentTypes, groupTypes);

            Assert.Equal(3, result.ToList().Count);
            var childItems = result.ToArray()[0].ChildItems;
            Assert.Equal("Establishment in Group 9 and 10", childItems[0].Name);
            Assert.Equal("Establishment in Group 10", childItems[1].Name);
            childItems = result.ToArray()[1].ChildItems;
            Assert.Equal("Establishment in Group 9 and 10", childItems[0].Name);
            Assert.Equal("Establishment in Group 9", childItems[1].Name);
            childItems = result.ToArray()[2].ChildItems;
            Assert.Empty(childItems);
        }
    }
}
