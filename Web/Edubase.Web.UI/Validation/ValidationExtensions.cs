using System.Web.Mvc;
using Edubase.Services.Domain;

namespace Edubase.Web.UI.Validation
{
    public static class ValidationExtensions
    {
        public static void ApplyToModelState(this ValidationEnvelopeDto validationEnvelope, ControllerContext controllerContext)
        {
            foreach (var error in validationEnvelope.Errors)
            {
                controllerContext.Controller.ViewData.ModelState.AddModelError(error.Fields, error.Message);
            }
        }
    }
}