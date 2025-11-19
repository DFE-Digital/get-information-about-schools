using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Edubase.Web.UI.Helpers.ModelBinding.BindingHandler;

/// <summary>
/// Base class for property binder handlers that supports chaining.
/// </summary>
/// <remarks>
/// This class implements the <em>Chain of Responsibility</em> pattern for property binding.
/// Each handler can attempt to bind a property, and if it cannot, it delegates to the next handler in the chain.
/// </remarks>
public abstract class PropertyBinderHandler : IPropertyBinderHandler
{
    private IPropertyBinderHandler _next;

    /// <summary>
    /// Sets the next handler in the chain.
    /// </summary>
    /// <param name="next">The next <see cref="IPropertyBinderHandler"/> to invoke if this handler cannot bind the property.</param>
    /// <returns>
    /// The handler that was set as the next in the chain, allowing fluent chaining of handlers.
    /// </returns>
    public IPropertyBinderHandler SetNext(IPropertyBinderHandler next)
    {
        _next = next;
        return next;
    }

    /// <summary>
    /// Attempts to handle binding for the specified property.
    /// </summary>
    /// <param name="context">The current <see cref="ModelBindingContext"/> containing binding information.</param>
    /// <param name="model">The model instance being bound.</param>
    /// <param name="property">The <see cref="PropertyInfo"/> representing the property to bind.</param>
    /// <returns>
    /// A task that resolves to <c>true</c> if the property was successfully bound by this handler or a subsequent handler,
    /// or <c>false</c> if no handler in the chain could bind the property.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="context"/>, <paramref name="model"/>, or <paramref name="property"/> is <c>null</c>.
    /// </exception>
    public virtual async Task<bool> HandleAsync(ModelBindingContext context, object model, PropertyInfo property)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(property);

        return _next != null &&
            await _next.HandleAsync(context, model, property);
    }
}
