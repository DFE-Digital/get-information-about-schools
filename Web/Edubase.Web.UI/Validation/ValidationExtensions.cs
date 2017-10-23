using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Edubase.Services.Domain;

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
                    controllerContext.Controller.ViewData.ModelState.AddModelError(error.Fields, error.GetMessage());
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