using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Edubase.Web.UI.Helpers.ModelBinding.BindingHandler;

/// <summary>
/// Common interface for property binder handlers in a chain of responsibility.
/// </summary>
public interface IPropertyBinderHandler
{
    /// <summary>
    /// Attempts to bind a property asynchronously.
    /// </summary>
    Task<bool> HandleAsync(ModelBindingContext context, object model, PropertyInfo property);

    /// <summary>
    /// Sets the next handler in the chain.
    /// </summary>
    IPropertyBinderHandler SetNext(IPropertyBinderHandler next);
}
