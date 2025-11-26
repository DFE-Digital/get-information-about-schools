using Edubase.Services.Enums;
using Edubase.Web.UI.Areas.Establishments.Models;
using Xunit;

namespace Edubase.Web.UIUnitTests.Areas.Establishments.Models
{
    public class TabWarningsModelTests
    {
        private const string AcademySecure16to19AddressWarning = "The address and location fields are representative of the care of address for safeguarding reasons.";

        [Theory]
        [InlineData((int) eLookupEstablishmentType.Academy1619Converter, "")]
        [InlineData(null, "")]
        [InlineData((int) eLookupEstablishmentType.AcademySecure16to19, AcademySecure16to19AddressWarning)]
        public void GivenEstablishmentType_TabWarningsSet(int? establishmentType, string expected)
        {
            var sut = new TabWarningsModel(establishmentType);

            Assert.Equal(expected, sut.LocationTab);
            Assert.Equal(expected, sut.DetailsTab);
        }
    }
}
