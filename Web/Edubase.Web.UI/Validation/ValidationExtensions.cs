using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Edubase.Services.Domain;

namespace Edubase.Web.UI.Validation
{
    public static class ValidationExtensions
    {
        private static string BuildFieldName(string errorFields, ControllerContext controllerContext)
        {
            var fieldName = errorFields;
            if (fieldName.Length > 0)
            {
                fieldName = string.Concat(errorFields.Substring(0, 1).ToUpper(),
                    errorFields.Substring(1, errorFields.Length - 1));
            }

            if (controllerContext.ModelState.Keys.Any(x =>
                    x.StartsWith(fieldName, StringComparison.InvariantCultureIgnoreCase)))
            {
                fieldName = controllerContext.ModelState.ContainsKey(fieldName)
                    ? controllerContext.ModelState.Keys.First(x =>
                        x.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase))
                    : controllerContext.ModelState.Keys
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
                    !controllerContext.ModelState.ContainsKey(error.Fields) ||
                    !controllerContext.ModelState[error.Fields].Errors.Any())
                {
                    var fieldName = BuildFieldName(error.Fields, controllerContext);
                    controllerContext.ModelState.AddModelError(fieldName, error.GetMessage());
                }
            }
        }

        public static void ApplyToModelState(this ValidationEnvelopeDto validationEnvelope,
            ControllerContext controllerContext, string baseProperty, bool avoidDuplicates = false)
        {
            foreach (var error in validationEnvelope.Errors)
            {
                if (!avoidDuplicates ||
                    !controllerContext.ModelState.ContainsKey($"{baseProperty}.{error.Fields}") ||
                    !controllerContext.ModelState[$"{baseProperty}.{error.Fields}"].Errors.Any())
                {
                    var fieldName = BuildFieldName(error.Fields, controllerContext);
                    controllerContext.ModelState.AddModelError($"{baseProperty}.{fieldName}",
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
                    controllerContext.ModelState.AddModelError(fieldName,
                        error.GetMessage() ??
                        "Error processing data");
                }
            }
        }
    }
}
