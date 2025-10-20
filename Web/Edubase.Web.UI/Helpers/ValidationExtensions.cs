using System.Globalization;
using System.Linq;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Edubase.Web.UI.Helpers
{
    public static class ValidationExtensions
    {
        public static void EduBaseAddToModelState(this ValidationResult result, ModelStateDictionary modelState, string prefix, bool avoidDuplicates = false)
        {
            if (result.IsValid)
            {
                return;
            }

            foreach (var error in result.Errors)
            {
                var key = string.IsNullOrEmpty(prefix) ? error.PropertyName : $"{prefix}.{error.PropertyName}";

                if (!avoidDuplicates ||
                    !modelState.ContainsKey(key) ||
                    !modelState[key].Errors.Any())
                {
                    modelState.AddModelError(key, error.CustomState?.ToString() ?? error.ErrorMessage);

                    // SetModelValue is available in ASP.NET Core, but ValueProviderResult requires proper construction
                    var attemptedValue = error.AttemptedValue ?? string.Empty;
                    modelState.SetModelValue(key, new ValueProviderResult(attemptedValue.ToString(), CultureInfo.CurrentCulture));
                }
            }
        }
    }
}
