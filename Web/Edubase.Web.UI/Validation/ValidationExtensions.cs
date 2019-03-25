using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Edubase.Services.Domain;
using Edubase.Common;
using Edubase.Common.Text;
using MoreLinq;

namespace Edubase.Web.UI.Validation
{
    public static class ValidationExtensions
    {
        public static void ApplyToModelState(this ValidationEnvelopeDto validationEnvelope, ControllerContext controllerContext, bool avoidDuplicates = false)
        {
            foreach (var error in validationEnvelope.Errors)
            {
                if (!avoidDuplicates ||
                    !controllerContext.Controller.ViewData.ModelState.ContainsKey(error.Fields) ||
                    !controllerContext.Controller.ViewData.ModelState[error.Fields].Errors.Any())
                {
                    // default the fieldname to TitleCase
                    var fieldName = error.Fields.ToTextCase(eTextCase.TitleCase);
                    // ensure the field name casing is not changed if it already exists within the model
                    if (controllerContext.Controller.ViewData.ModelState.Keys.Any(x => x.StartsWith(fieldName, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        fieldName = controllerContext.Controller.ViewData.ModelState.Keys.Contains(fieldName, StringComparer.InvariantCultureIgnoreCase) ?
                            controllerContext.Controller.ViewData.ModelState.Keys.First(x => x.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase)) :
                            controllerContext.Controller.ViewData.ModelState.Keys.First(x => x.StartsWith(fieldName, StringComparison.InvariantCultureIgnoreCase)).Substring(0, fieldName.Length);
                    }
                    controllerContext.Controller.ViewData.ModelState.AddModelError(fieldName, error.GetMessage());
                }
            }
        }

        public static void ApplyToModelState(this ValidationEnvelopeDto validationEnvelope, ControllerContext controllerContext, string baseProperty)
        {
            foreach (var error in validationEnvelope.Errors)
            {
                controllerContext.Controller.ViewData.ModelState.AddModelError($"{baseProperty}.{error.Fields}", error.GetMessage());
            }
        }

        public static void ApplyToModelState(this ApiResponse apiResponse, ControllerContext controllerContext)
        {
            if (apiResponse.HasErrors && apiResponse.Errors != null)
            {
                foreach (var error in apiResponse.Errors)
                {
                    controllerContext.Controller.ViewData.ModelState.AddModelError(error.Fields, error.GetMessage());
                }
            }
        }
    }
}
