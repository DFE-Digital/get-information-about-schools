using System;
using System.Linq;
using System.Web.Mvc;
using Edubase.Services.Domain;
using FluentValidation.Results;

namespace Edubase.Web.UI.Validation
{
    public static class ValidationExtensions
    {
        private static string BuildFieldName(string errorFields, ControllerContext controllerContext)
        {
            // correct the naming convention by upper casing the first letter
            var fieldName = errorFields;
            if (fieldName.Length > 0)
            {
                fieldName = string.Concat(errorFields.Substring(0, 1).ToUpper(),
                    errorFields.Substring(1, errorFields.Length - 1));
            }

            // as we're adding this - we want to use the same casing as the other properties follow. Because of that
            // - look to see if there are any others which extend the original name
            if (controllerContext.Controller.ViewData.ModelState.Keys.Any(x =>
                    x.StartsWith(fieldName, StringComparison.InvariantCultureIgnoreCase)))
            {
                fieldName = controllerContext.Controller.ViewData.ModelState.ContainsKey(fieldName)
                    ? controllerContext.Controller.ViewData.ModelState.Keys.First(x =>
                        x.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase))
                    : controllerContext.Controller.ViewData.ModelState.Keys
                        .First(x => x.StartsWith(fieldName, StringComparison.InvariantCultureIgnoreCase))
                        .Substring(0, fieldName.Length);
            }

            return fieldName;
        }

        public static void ApplyToModelState(this ValidationEnvelopeDto validationEnvelope,
            ControllerContext controllerContext, bool avoidDuplicates = false)
        {
            foreach (var error in validationEnvelope.Errors)
            {
                if (!avoidDuplicates ||
                    !controllerContext.Controller.ViewData.ModelState.ContainsKey(error.Fields) ||
                    !controllerContext.Controller.ViewData.ModelState[error.Fields].Errors.Any())
                {
                    var fieldName = BuildFieldName(error.Fields, controllerContext);
                    controllerContext.Controller.ViewData.ModelState.AddModelError(fieldName, error.GetMessage());
                }
            }
        }

        public static void ApplyToModelState(this ValidationEnvelopeDto validationEnvelope,
            ControllerContext controllerContext, string baseProperty, bool avoidDuplicates = false)
        {
            foreach (var error in validationEnvelope.Errors)
            {
                if (!avoidDuplicates ||
                    !controllerContext.Controller.ViewData.ModelState.ContainsKey($"{baseProperty}.{error.Fields}") ||
                    !controllerContext.Controller.ViewData.ModelState[$"{baseProperty}.{error.Fields}"].Errors.Any())
                {
                    var fieldName = BuildFieldName(error.Fields, controllerContext);
                    controllerContext.Controller.ViewData.ModelState.AddModelError($"{baseProperty}.{fieldName}",
                        error.GetMessage());
                }
            }
        }

        public static void ApplyToModelState(this ApiResponse apiResponse, ControllerContext controllerContext)
        {
            if (apiResponse.HasErrors && apiResponse.Errors != null)
            {
                foreach (var error in apiResponse.Errors)
                {
                    var fieldName = BuildFieldName(error.Fields, controllerContext);
                    controllerContext.Controller.ViewData.ModelState.AddModelError(fieldName,
                        error.GetMessage() ??
                        "Error processing data"); //Generic message provided to prevent system crashing on no error message
                }
            }
        }

        public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState, string prefix)
        {
            foreach (var error in result.Errors)
            {
                var key = string.IsNullOrEmpty(prefix) ? error.PropertyName : $"{prefix}.{error.PropertyName}";
                modelState.AddModelError(key, error.ErrorMessage);
            }
        }
    }
}
