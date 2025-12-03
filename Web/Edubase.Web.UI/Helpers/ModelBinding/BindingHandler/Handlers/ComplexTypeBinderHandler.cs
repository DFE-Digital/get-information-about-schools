using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Edubase.Web.UI.Helpers.ModelBinding.BindingHandler.Handlers.Extensions;
using Edubase.Web.UI.Helpers.ModelBinding.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace Edubase.Web.UI.Helpers.ModelBinding.BindingHandler.Handlers;

/// <summary>
/// A binder handler that manages binding of complex types, including arrays of complex types,
/// by delegating to the appropriate model binders.
/// </summary>
/// <remarks>
/// This handler is part of the property binder chain and is responsible for handling
/// non-simple types. It explicitly supports arrays of complex types and delegates
/// other complex type binding to the default model binder.
/// </remarks>
/// <param name="metadataProvider">The metadata provider used to obtain model metadata.</param>
public sealed class ComplexTypeBinderHandler(IModelMetadataProvider metadataProvider) : PropertyBinderHandler
{
    private readonly IModelMetadataProvider _metadataProvider = metadataProvider;

    /// <summary>
    /// Attempts to bind a property when its type is a complex type.
    /// </summary>
    /// <param name="context">The current <see cref="ModelBindingContext"/> containing metadata, value providers, and binding state.</param>
    /// <param name="model">The model instance whose property is being bound.</param>
    /// <param name="property">The <see cref="PropertyInfo"/> representing the property to bind.</param>
    /// <returns>
    /// A task that resolves to <c>true</c> if the property was successfully bound,
    /// or <c>false</c> if binding was delegated to the next handler in the chain.
    /// </returns>
    public override async Task<bool> HandleAsync(
        ModelBindingContext context, object model, PropertyInfo property)
    {
        // Skip indexers and non-settable properties
        if (property.GetIndexParameters().Length > 0 ||
            !property.CanWrite ||
            property.GetSetMethod() == null)
        {
            return await base.HandleAsync(context, model, property);
        }

        // Skip simple types
        if (property.PropertyType.IsSimpleType())
        {
            return await base.HandleAsync(context, model, property);
        }

        // Handle arrays of complex types explicitly
        if (property.PropertyType.IsArray)
        {
            Type? elementType = property.PropertyType.GetElementType();

            if (elementType != null && !elementType.IsSimpleType())
            {
                if (await BindArrayAsync(context, model, property, elementType))
                {
                    return true;
                }
            }
        }

        // Fallback: let default binder handle other complex types
        return await BindNestedAsync(context, model, property);
    }

    /// <summary>
    /// Binds an array property by iterating over indexed values in the value provider
    /// and creating instances of the element type using the default binder.
    /// </summary>
    /// <param name="context">The current binding context.</param>
    /// <param name="model">The model instance being bound.</param>
    /// <param name="property">The array property to bind.</param>
    /// <param name="elementType">The type of elements in the array.</param>
    /// <returns><c>true</c> if the array was successfully bound; otherwise <c>false</c>.</returns>
    private async Task<bool> BindArrayAsync(
        ModelBindingContext context, object model, PropertyInfo property, Type elementType)
    {
        string propertyPrefix = context.BuildPropertyPrefix(property);

        IModelBinder binder =
            CreateBinder(context, elementType);

        ModelMetadata metadata =
            _metadataProvider.GetMetadataForType(elementType);

        List<object> items = [];
        int index = 0;

        // Iterate over indexed values until no more values are found
        while (HasValuesForIndex(context, propertyPrefix, index, elementType))
        {
            string elementPrefix =
                propertyPrefix.BuildElementPrefix(index);

            DefaultModelBindingContext elementContext =
                CreateBindingContext(context, metadata, elementPrefix);

            await binder.BindModelAsync(elementContext);

            if (elementContext.Result.IsModelSet &&
                elementContext.Result.Model != null)
            {
                items.Add(elementContext.Result.Model);
            }

            index++;
        }

        // If items were bound, assign them to the property
        if (items.Count > 0)
        {
            Array array = Array.CreateInstance(elementType, items.Count);
            items.CopyTo((object[]) array, 0);
            property.SetValue(model, array);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Binds a nested complex property using the default binder.
    /// </summary>
    /// <param name="context">The current binding context.</param>
    /// <param name="model">The model instance being bound.</param>
    /// <param name="property">The complex property to bind.</param>
    /// <returns><c>true</c> if the property was successfully bound; otherwise <c>false</c>.</returns>
    private async Task<bool> BindNestedAsync(
        ModelBindingContext context, object model, PropertyInfo property)
    {
        string propertyPrefix = context.BuildPropertyPrefix(property);

        ModelMetadata metadata =
            _metadataProvider.GetMetadataForType(property.PropertyType);

        IModelBinder binder =
            CreateBinder(context, property.PropertyType);

        DefaultModelBindingContext nestedContext =
            CreateBindingContext(context, metadata, propertyPrefix);

        await binder.BindModelAsync(nestedContext);

        if (nestedContext.Result.IsModelSet &&
            nestedContext.Result.Model != null &&
            property.PropertyType
                .IsAssignableFrom(nestedContext.Result.Model.GetType()))
        {
            property.SetValue(model, nestedContext.Result.Model);
            return true;
        }

        return await base.HandleAsync(context, model, property);
    }

    /// <summary>
    /// Creates a model binder for the specified type using the <see cref="IModelBinderFactory"/>.
    /// </summary>
    /// <param name="context">The current binding context.</param>
    /// <param name="type">The type for which to create a binder.</param>
    /// <returns>An <see cref="IModelBinder"/> capable of binding the specified type.</returns>
    private static IModelBinder CreateBinder(ModelBindingContext context, Type type)
    {
        IModelBinderFactory factory =
            context.ActionContext.HttpContext.RequestServices
                .GetRequiredService<IModelBinderFactory>();

        IModelMetadataProvider metadataProvider =
            context.ActionContext.HttpContext.RequestServices
                .GetRequiredService<IModelMetadataProvider>();

        return factory.CreateBinder(
            new ModelBinderFactoryContext
            {
                Metadata = metadataProvider.GetMetadataForType(type),
                CacheToken = type
            });
    }

    /// <summary>
    /// Creates a new <see cref="DefaultModelBindingContext"/> for the given type and model name.
    /// </summary>
    /// <param name="context">The parent binding context.</param>
    /// <param name="metadata">The metadata for the type being bound.</param>
    /// <param name="modelName">The name of the model being bound.</param>
    /// <returns>A new <see cref="DefaultModelBindingContext"/> configured for the given type.</returns>
    private static DefaultModelBindingContext CreateBindingContext(
        ModelBindingContext context,
        ModelMetadata metadata,
        string modelName) =>
        (DefaultModelBindingContext) DefaultModelBindingContext.CreateBindingContext(
            context.ActionContext,
            context.ValueProvider,
            metadata,
            bindingInfo: null,
            modelName: modelName);

    /// <summary>
    /// Determines whether the value provider contains values for the specified array index.
    /// </summary>
    /// <param name="context">The current binding context.</param>
    /// <param name="prefix">The property prefix used to build keys.</param>
    /// <param name="index">The array index to check.</param>
    /// <param name="elementType">The type of elements in the array.</param>
    /// <returns><c>true</c> if values exist for the given index; otherwise <c>false</c>.</returns>
    private static bool HasValuesForIndex(
        ModelBindingContext context, string prefix, int index, Type elementType)
    {
        string elementPrefix = prefix.BuildElementPrefix(index);

        foreach (PropertyInfo property in
            elementType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            string key =
                elementPrefix.BuildElementPropertyKey(property);

            if (context.ValueProvider.HasValue(key))
            {
                return true;
            }
        }

        return false;
    }
}
