using System.Reflection;
using System.Threading.Tasks;
using Edubase.Web.UI.Helpers.ModelBinding.BindingHandler;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Edubase.Web.UIUnitTests.Helpers.ModelBinding.TestDoubles;

/// <summary>
/// Test double implementation of <see cref="IPropertyBinderHandler"/>.
/// Records whether it was called and which property was handled.
/// </summary>
internal class PropertyBinderHandlerTestDoubles : IPropertyBinderHandler
{
    /// <summary>
    /// Gets a value indicating whether <see cref="HandleAsync"/> was invoked.
    /// </summary>
    public bool WasCalled { get; private set; }

    /// <summary>
    /// Gets the last property passed to <see cref="HandleAsync"/>.
    /// </summary>
    public PropertyInfo LastProperty { get; private set; }

    private IPropertyBinderHandler _next;

    /// <summary>
    /// Always returns false so that <see cref="DefaultModelBinder"/> continues binding.
    /// </summary>
    public static Task<bool> BindModelAsync(
        ModelBindingContext context) => Task.FromResult(false);

    /// <summary>
    /// Marks the handler as called and records the property.
    /// </summary>
    public Task<bool> HandleAsync(
        ModelBindingContext context,
        object model, PropertyInfo property)
    {
        WasCalled = true;
        LastProperty = property;
        return Task.FromResult(true);
    }

    /// <summary>
    /// Stores the next handler in the chain.
    /// </summary>
    public IPropertyBinderHandler SetNext(
        IPropertyBinderHandler next)
    {
        _next = next;
        return next;
    }
}

