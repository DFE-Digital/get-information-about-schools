using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Edubase.Web.UI.Filters
{
    public abstract class ModelStateTransfer : ActionFilterAttribute
    {
        protected static readonly string Key = typeof(ModelStateTransfer).FullName;
    }

    public class ExportModelStateAttribute : ModelStateTransfer
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var controller = context.Controller as Controller;
            if (controller == null) return;

            if (!controller.ViewData.ModelState.IsValid &&
                (context.Result is RedirectResult || context.Result is RedirectToRouteResult))
            {
                controller.TempData[Key] = controller.ViewData.ModelState;
            }

            base.OnActionExecuted(context);
        }
    }

    public class ImportModelStateAttribute : ModelStateTransfer
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var controller = context.Controller as Controller;
            if (controller == null) return;

            if (controller.TempData.TryGetValue(Key, out var value) && value is ModelStateDictionary modelState)
            {
                if (context.Result is ViewResult)
                {
                    controller.ViewData.ModelState.Merge(modelState);
                }
                else
                {
                    controller.TempData.Remove(Key);
                }
            }

            base.OnActionExecuted(context);
        }
    }
}
