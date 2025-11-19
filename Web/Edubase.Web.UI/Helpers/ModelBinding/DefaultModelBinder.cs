using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Edubase.Web.UI.Helpers.ModelBinding.Extensions;
using Edubase.Web.UI.Helpers.ModelBinding.Factories;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Edubase.Web.UI.Helpers.ModelBinding.BindingHandler;

/// <summary>
/// Default model binder that orchestrates binding using a chain of property binder handlers.
/// </summary>
/// <remarks>
/// This binder implements the <em>Chain of Responsibility</em> pattern.
/// It delegates binding of individual properties to a sequence of <see cref="IPropertyBinderHandler"/> instances.
/// For top-level simple models (e.g. string, int), it delegates binding to the injected
/// <see cref="SimpleTypeBinderHandler"/>.
/// </remarks>
public class DefaultModelBinder : IModelBinder
{
    private readonly IPropertyBinderHandler _handlerChain;
    private readonly ITypeFactory _factory;
    private readonly SimpleTypeBinderHandler _simpleHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultModelBinder"/> class.
    /// </summary>
    /// <param name="handlers">The collection of property binder handlers to chain together.</param>
    /// <param name="factory">The type factory used to create model instances.</param>
    /// <exception cref="InvalidOperationException">Thrown if no handlers are registered.</exception>
    public DefaultModelBinder(
        IEnumerable<IPropertyBinderHandler> handlers, ITypeFactory factory)
    {
        _factory = factory;

        IPropertyBinderHandler current = null;

        foreach (IPropertyBinderHandler handler in handlers)
        {
            if (handler is SimpleTypeBinderHandler simple)
            {
                _simpleHandler = simple;
            }

            current = current == null
                ? (_handlerChain = handler)
                : current.SetNext(handler);
        }

        if (_handlerChain == null)
        {
            throw new InvalidOperationException(
                "No property binder handlers were registered.");
        }
    }

    /// <summary>
    /// Binds a model instance for the given <see cref="ModelBindingContext"/>.
    /// </summary>
    /// <param name="bindingContext">The binding context containing metadata and value providers.</param>
    /// <returns>A task representing the asynchronous binding operation.</returns>
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        Type modelType =
            bindingContext.ModelType.ResolveConcreteType();

        // Delegate top-level simple models to the injected SimpleTypeBinderHandler.
        if (_simpleHandler != null &&
            await _simpleHandler.BindModelAsync(bindingContext))
        {
            return;
        }

        // Complex type binding
        object modelInstance = null!;

        try
        {
            modelInstance = _factory.CreateInstance(modelType);
        }
        catch (Exception)
        {
            bindingContext.Result = ModelBindingResult.Failed();
        }

        if (modelInstance != null)
        {
            foreach (PropertyInfo property in
                modelType.GetProperties(
                    BindingFlags.Public | BindingFlags.Instance))
            {
                await _handlerChain.HandleAsync(
                    bindingContext, modelInstance, property);
            }
        }

        bindingContext.Result =
            ModelBindingResult.Success(modelInstance);
    }
}
