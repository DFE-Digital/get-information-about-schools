using System.Collections.Generic;
using Edubase.Web.UI.Areas.Governors.Models;
using Edubase.Web.UI.Areas.Governors.Models.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace Edubase.Web.UIUnitTests.Areas.Governors.Models
{
    public class EditGroupCorporateContactViewModelValidatorTests
    {
        private readonly EditGroupCorporateContactViewModelValidator _validator =
            new EditGroupCorporateContactViewModelValidator();

        public static IEnumerable<object[]> TestCases =>
            new[]
            {
                new object[] { null, true },
                new object[] { "", true },
                new object[] { new string('x', 150), true },
                new object[] { new string('x', 151), false, "Must be 150 characters or less" }
            };

        [Theory]
        [MemberData(nameof(TestCases))]
        public void CorporateContact_Validation_ReturnsCorrectResult(string name, bool expectedResult, string expectedMessage = null)
        {
            // Arrange
            var model = new EditGroupCorporateContactViewModel
            {
                CorporateContact = name
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            if (expectedResult)
            {
                result.ShouldNotHaveAnyValidationErrors();
            }
            else
            {
                result.ShouldHaveValidationErrorFor(x => x.CorporateContact)
                  .WithErrorMessage(expectedMessage);
            }
        }        
    }
}
