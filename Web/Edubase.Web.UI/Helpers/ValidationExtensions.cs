using FluentValidation.Results;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

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
                    modelState.SetModelValue(key, new ValueProviderResult(error.AttemptedValue ?? "", (error.AttemptedValue ?? "").ToString(), CultureInfo.CurrentCulture));
                }
            }
        }
    }
}
