using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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
                var key = string.IsNullOrEmpty(prefix) ? error.PropertyName : prefix + "." + error.PropertyName;

                if (!avoidDuplicates ||
                    !modelState.ContainsKey(key) ||
                    !modelState[key].Errors.Any())
                {
                    modelState.AddModelError(key, error.CustomState?.ToString() ?? error.ErrorMessage);
                    modelState.SetModelValue(key, new ValueProviderResult(error.AttemptedValue?.ToString() ?? string.Empty));
                }
            }
        }

        public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState, string prefix)
            => result.EduBaseAddToModelState(modelState, prefix);

        public static bool IsValidField(this ModelStateDictionary modelState, string key)
            => modelState.GetFieldValidationState(key) != ModelValidationState.Invalid;
    }
}
