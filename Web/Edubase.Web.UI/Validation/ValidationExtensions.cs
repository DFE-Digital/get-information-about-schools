using System;
using System.Linq;
using Edubase.Services.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Edubase.Web.UI.Validation
{
    public static class ValidationExtensions
    {
        private static string BuildFieldName(string errorFields, ControllerContext controllerContext)
        {
            if (string.IsNullOrWhiteSpace(errorFields))
                return errorFields;

            // Capitalize first letter
            var fieldName = char.ToUpper(errorFields[0]) + errorFields.Substring(1);

            var modelState = controllerContext.ModelState;

            // Match casing with existing keys in ModelState
            if (modelState.Keys.Any(x => x.StartsWith(fieldName, StringComparison.InvariantCultureIgnoreCase)))
            {
                fieldName = modelState.ContainsKey(fieldName)
                    ? modelState.Keys.First(x => x.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase))
                    : modelState.Keys.First(x => x.StartsWith(fieldName, StringComparison.InvariantCultureIgnoreCase))
                        .Substring(0, fieldName.Length);
            }

            return fieldName;
        }

        public static void ApplyToModelState(this ValidationEnvelopeDto validationEnvelope,
            ControllerContext controllerContext, bool avoidDuplicates = false)
        {
            var modelState = controllerContext.ModelState;

            foreach (var error in validationEnvelope.Errors)
            {
                if (!avoidDuplicates ||
                    !modelState.ContainsKey(error.Fields) ||
                    !modelState[error.Fields].Errors.Any())
                {
                    var fieldName = BuildFieldName(error.Fields, controllerContext);
                    modelState.AddModelError(fieldName, error.GetMessage());
                }
            }
        }

        public static void ApplyToModelState(this ValidationEnvelopeDto validationEnvelope,
            ControllerContext controllerContext, string baseProperty, bool avoidDuplicates = false)
        {
            var modelState = controllerContext.ModelState;

            foreach (var error in validationEnvelope.Errors)
            {
                var fullKey = $"{baseProperty}.{error.Fields}";

                if (!avoidDuplicates ||
                    !modelState.ContainsKey(fullKey) ||
                    !modelState[fullKey].Errors.Any())
                {
                    var fieldName = BuildFieldName(error.Fields, controllerContext);
                    modelState.AddModelError($"{baseProperty}.{fieldName}", error.GetMessage());
                }
            }
        }

        public static void ApplyToModelState(this ApiResponse apiResponse, ControllerContext controllerContext)
        {
            if (apiResponse.HasErrors && apiResponse.Errors != null)
            {
                var modelState = controllerContext.ModelState;

                foreach (var error in apiResponse.Errors)
                {
                    var fieldName = BuildFieldName(error.Fields, controllerContext);
                    modelState.AddModelError(fieldName, error.GetMessage() ?? "Error processing data");
                }
            }
        }
    }
}
