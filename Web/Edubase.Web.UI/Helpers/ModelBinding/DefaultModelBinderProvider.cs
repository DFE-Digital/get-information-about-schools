using Edubase.Web.UI.Helpers.ModelBinding.BindingHandler;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Edubase.Web.UI.Helpers.ModelBinding;

/// <summary>
/// Provides a custom model binder to the ASP.NET Core model binding system.
/// Defers resolution of <see cref="DefaultModelBinder"/> to the current request scope.
/// </summary>
public class DefaultModelBinderProvider : IModelBinderProvider
{
    /// <summary>
    /// Returns an instance of <see cref="IModelBinder"/> for the given binding context.
    /// </summary>
    /// <param name="context">The model binder provider context.</param>
    /// <returns>
    /// A binder that resolves <see cref="DefaultModelBinder"/> from the current request services.
    /// </returns>
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        // Allow the built-in body binder to handle Json requests(eg FromBody paramters)
        if (context.BindingInfo.BindingSource?.CanAcceptDataFrom(BindingSource.Body) == true)
        {
            return null;
        }

        return new BinderTypeModelBinder(typeof(DefaultModelBinder));
    }
}
