using System;
using System.Linq;
using Edubase.Services.Enums;
using Edubase.Services.Establishments.Search;
using Edubase.Web.UI.Helpers;
using Edubase.Web.UIUnitTests.Helpers.Data;
using Xunit;

namespace Edubase.Web.UIUnitTests.Helpers
{
    public class SecureAcademyUtilityTestsI
    {
        [Theory]
        [InlineData(null, false, "Manage academy openings")]
        [InlineData("", false, "Manage academy openings")]
        [InlineData(" ", false, "Manage academy openings")]
        [InlineData(null, true, "Manage academy openings")]
        [InlineData("", true, "Manage academy openings")]
        [InlineData(" ", true, "Manage academy openings")]
        [InlineData("46", true, "Manage secure academy 16-19 openings")]
        public void GetAcademyOpeningPageTitle_WhenCalledWithValidValues_ReturnsCorrectTitle(string establishmentTypeId,
            bool isSecure16To19User, string expectedPageTitle)
        {
            var result = AcademyUtility.GetAcademyOpeningPageTitle(establishmentTypeId, isSecure16To19User);

            Assert.Equal(result, expectedPageTitle);
        }

        [Theory]
        [InlineData("48", true)]
        [InlineData("46", false)]
        public void GetAcademyOpeningPageTitle_WhenCalledWithInvalidValues_ThrowsArgumentException(
            string establishmentTypeId, bool isSecure16To19User) =>
            Assert.Throws<ArgumentException>(() =>
                AcademyUtility.GetAcademyOpeningPageTitle(establishmentTypeId, isSecure16To19User));


        [Theory]
        [InlineData(true, "46", 2)]
        [InlineData(false, "46", 6)]
        [InlineData(true, "48", 6)]
        [InlineData(false, null, 6)]
        [InlineData(false, "", 6)]
        [InlineData(false, " ", 6)]
        [InlineData(true, null, 6)]
        [InlineData(true, "", 6)]
        [InlineData(true, " ", 6)]
        public void FilterEstablishmentsByEstablishmentTypeId_WhenCalled_ReturnsCorrectlyFilteredEstablishment
            (bool isSecure16To19User, string establishmentTypeId, int extectedResult)
        {
            var establishments = DataUtility.GetEstablishmentLookupDto();

            var result = AcademyUtility.FilterEstablishmentsByEstablishmentTypeId
                (establishments, establishmentTypeId, isSecure16To19User);

            Assert.Equal(result.Count(), extectedResult);
        }

        [Theory]
        [InlineData(true, "45")]
        [InlineData(false, "45")]
        [InlineData(false, "46")]
        public void GetEstablishmentSearchFilters_WhenCalledWithInvalidValues_ThrowsArgumentException(
            bool isSecure16To19User, string establishmentTypeId)
        {
            var parameter = new GetEstabSearchFiltersParam(DateTime.Now.Date.AddDays(-1), DateTime.Now,
                establishmentTypeId, isSecure16To19User);

            Assert.Throws<ArgumentException>(() => AcademyUtility.GetEstablishmentSearchFilters(parameter));
        }

        [Theory]
        [InlineData(true, "46")]
        public void GetEstablishmentSearchFilters_WhenCalledWithValidSecure16To19Values_ReturnsExpectedFilters(
            bool isSecure16To19User, string establishmentTypeId)
        {
            var parameter = new GetEstabSearchFiltersParam(DateTime.Now.Date.AddDays(-1), DateTime.Now,
                establishmentTypeId, isSecure16To19User);

            var result = AcademyUtility.GetEstablishmentSearchFilters(parameter);

            Assert.IsType<EstablishmentSearchFilters>(result);
            Assert.Empty(result.EstablishmentTypeGroupIds);
            Assert.Single(result.TypeIds);
            Assert.Equal(result.TypeIds.FirstOrDefault(), int.Parse(establishmentTypeId));
        }

        [Theory]
        [InlineData(false, null)]
        [InlineData(false, "")]
        [InlineData(false, " ")]
        public void GetEstablishmentSearchFilters_WhenCalledWithValidNonSecure16To19Values_ReturnsExpectedFilters(
            bool isSecure16To19User, string establishmentTypeId)
        {
            var parameter = new GetEstabSearchFiltersParam(DateTime.Now.Date.AddDays(-1), DateTime.Now,
                establishmentTypeId, isSecure16To19User);

            var result = AcademyUtility.GetEstablishmentSearchFilters(parameter);

            Assert.IsType<EstablishmentSearchFilters>(result);
            Assert.Empty(result.TypeIds);
            Assert.Single(result.EstablishmentTypeGroupIds);
            Assert.Equal((int) eLookupEstablishmentTypeGroup.Academies,
                result.EstablishmentTypeGroupIds.FirstOrDefault());
        }

        [Theory]
        [InlineData("", "A03pdA0yBdWaKdpvHY0E2Q==")]
        [InlineData(" ", "KfaJ1dPpVO5DrAUD+09UZA==")]
        [InlineData("test", "tiEUR7hmOoxqNzQ9d5reWQ==")]
        [InlineData("46", "W3XrRxuGnzvRuqxllYzLlg==")]
        public void EncryptValue_WhenCalled_ReturnsEncryptedValue(string value, string expectedValue)
        {
            var result = AcademyUtility.EncryptValue(value);

            Assert.NotEmpty(result);
            Assert.Equal(result, expectedValue);
        }

        [Theory]
        [InlineData("KfaJ1dPpVO5DrAUD+09UZA==", " ")]
        [InlineData("tiEUR7hmOoxqNzQ9d5reWQ==", "test")]
        [InlineData("A03pdA0yBdWaKdpvHY0E2Q==", "")]
        public void DecryptValue_WhenCalled_ReturnsDecryptedValue(string value, string expectedValue)
        {
            var result = AcademyUtility.DecryptValue(value);

            Assert.Equal(result, expectedValue);
        }

        [Theory]
        [InlineData("KfaJ1dPpVO5DrAUD+09UZl==")]
        [InlineData("A03qdA0yBdWaKdpvHY0E2Q==")]
        public void DecryptValue_WhenCalledWithAnInvalidValue_ThrowsArgumentException(string value) =>
            Assert.Throws<ArgumentException>(() => AcademyUtility.DecryptValue(value));
    }
}
