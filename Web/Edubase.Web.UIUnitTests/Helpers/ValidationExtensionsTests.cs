using Edubase.Web.UI.Helpers;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Xunit;

namespace Edubase.Web.UIUnitTests.Helpers
{
    public class ValidationExtensionsTests
    {
        [Fact]
        public void AddToModelState_PreservesSubmittedValuesAndExistingErrors()
        {
            var modelState = new ModelStateDictionary();
            modelState.SetModelValue("Name", "Submitted name", "Submitted name");
            modelState.AddModelError("Name", "Existing error");
            var result = new ValidationResult(
            [
                new ValidationFailure("Name", "First validation error")
                {
                    AttemptedValue = "Different name",
                    CustomState = "Custom message"
                },
                new ValidationFailure("Name", "Second validation error")
            ]);

            result.AddToModelState(modelState, string.Empty);

            Assert.Equal("Submitted name", modelState["Name"].AttemptedValue);
            Assert.Collection(modelState["Name"].Errors,
                error => Assert.Equal("Existing error", error.ErrorMessage),
                error => Assert.Equal("First validation error", error.ErrorMessage),
                error => Assert.Equal("Second validation error", error.ErrorMessage));
        }

        [Theory]
        [InlineData(null, "Name", "Name")]
        [InlineData("", "Name", "Name")]
        [InlineData("Group", "Name", "Group.Name")]
        [InlineData("Group", "", "Group")]
        public void AddToModelState_UsesPropertyNameAndPrefix(string prefix, string propertyName, string expectedKey)
        {
            var modelState = new ModelStateDictionary();
            var result = new ValidationResult(
            [
                new ValidationFailure(propertyName, "Validation error")
            ]);

            result.AddToModelState(modelState, prefix);

            var entry = Assert.Single(modelState);
            Assert.Equal(expectedKey, entry.Key);
            Assert.Equal("Validation error", Assert.Single(entry.Value.Errors).ErrorMessage);
        }
    }
}
