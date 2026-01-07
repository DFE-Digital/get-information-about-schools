using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace Edubase.Web.UI.Helpers.ModelBinding.BindingHandler.Handlers.Extensions;

/// <summary>
/// Provides extension methods to simplify common string manipulation and value lookups
/// used during model binding.
/// </summary>
public static class ModelBindingExtensions
{
    /// <summary>
    /// Builds a property prefix from the current model name and property.
    /// </summary>
    /// <param name="context">The current model binding context.</param>
    /// <param name="property">The property for which to build the prefix.</param>
    /// <returns>
    /// A string representing the property prefix, e.g. "Parent.PropertyName" or just "PropertyName"
    /// if the model name is empty.
    /// </returns>
    public static string BuildPropertyPrefix(
        this ModelBindingContext context, PropertyInfo property) =>
            string.IsNullOrEmpty(context.ModelName)
                ? property.Name
                : $"{context.ModelName}.{property.Name}";

    /// <summary>
    /// Creates a model binder capable of binding the specified element type.
    /// </summary>
    /// <param name="context">The parent model binding context.</param>
    /// <param name="type">The element type to bind.</param>
    /// <returns>An <see cref="IModelBinder"/> for the element type.</returns>
    public static IModelBinder CreateBinder(
        this ModelBindingContext context, Type type)
    {
        IModelBinderFactory factory =
            context.ActionContext.HttpContext.RequestServices
            .GetRequiredService<IModelBinderFactory>();

        return factory.CreateBinder(
            new ModelBinderFactoryContext
            {
                Metadata = context.GetMetadata(type),
                CacheToken = type
            });
    }

    /// <summary>
    /// Retrieves model metadata for the specified type.
    /// </summary>
    /// <param name="context">The current model binding context.</param>
    /// <param name="type">The type whose metadata is required.</param>
    /// <returns>The <see cref="ModelMetadata"/> for the type.</returns>
    public static ModelMetadata GetMetadata(
        this ModelBindingContext context, Type type)
    {
        IModelMetadataProvider provider =
            context.ActionContext.HttpContext.RequestServices
            .GetRequiredService<IModelMetadataProvider>();

        return provider.GetMetadataForType(type);
    }

    /// <summary>
    /// Creates a new <see cref="DefaultModelBindingContext"/> for binding a nested or array element.
    /// </summary>
    /// <param name="parent">The parent binding context.</param>
    /// <param name="metadata">The metadata for the element type.</param>
    /// <param name="modelName">The model name prefix for the element.</param>
    /// <returns>A configured <see cref="DefaultModelBindingContext"/>.</returns>
    public static DefaultModelBindingContext CreateBindingContext(
        this ModelBindingContext context,
        ModelMetadata metadata,
        string modelName) =>
            (DefaultModelBindingContext)
                DefaultModelBindingContext.CreateBindingContext(
                    context.ActionContext,
                    context.ValueProvider,
                    metadata,
                    bindingInfo: null,
                    modelName: modelName);

    /// <summary>
    /// Enumerates indexed prefixes such as <c>Property[0]</c>, <c>Property[1]</c>, etc.
    /// </summary>
    /// <param name="context">The current model binding context.</param>
    /// <param name="basePrefix">The base prefix for the array property.</param>
    /// <param name="elementType">The type of elements in the array.</param>
    /// <returns>An enumerable sequence of element prefixes.</returns>
    public static IEnumerable<string> EnumerateIndexedPrefixes(
        this ModelBindingContext context, string basePrefix, Type elementType)
    {
        int index = 0;

        while (true)
        {
            if (!HasValuesForIndex(context, basePrefix, index, elementType))
            {
                yield break;
            }

            yield return $"{basePrefix}[{index}]";
            index++;
        }
    }

    /// <summary>
    /// Determines whether the value provider contains values for a specific array index.
    /// </summary>
    /// <param name="context">The current model binding context.</param>
    /// <param name="prefix">The base prefix for the array property.</param>
    /// <param name="index">The array index to check.</param>
    /// <param name="elementType">The type of elements in the array.</param>
    /// <returns>
    /// <c>true</c> if values exist for the specified index; otherwise <c>false</c>.
    /// </returns>
    private static bool HasValuesForIndex(
        ModelBindingContext context, string prefix, int index, Type elementType)
    {
        string elementPrefix = BuildElementPrefix(prefix, index);

        foreach (PropertyInfo property in
            elementType.GetProperties(
                BindingFlags.Public | BindingFlags.Instance))
        {
            string key = BuildElementPropertyKey(elementPrefix, property);

            if (HasValue(context.ValueProvider, key))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Enumerates element prefixes such as <c>Property[0]</c>, <c>Property[1]</c>, etc.,
    /// stopping when no matching keys exist in the value provider.
    /// </summary>
    /// <param name="context">The current model binding context.</param>
    /// <param name="basePrefix">The base prefix for the list property.</param>
    /// <returns>An enumerable sequence of element prefixes.</returns>
    public static IEnumerable<string> EnumerateElementPrefixes(
        this ModelBindingContext context,
        string basePrefix)
    {
        int index = 0;

        while (true)
        {
            string prefix = $"{basePrefix}[{index}]";

            if (!context.ValueProvider.ContainsPrefix(prefix))
            {
                yield break;
            }

            yield return prefix;
            index++;
        }
    }

    /// <summary>
    /// Builds an element prefix for an array or list index.
    /// </summary>
    /// <param name="propertyPrefix">The property prefix of the collection.</param>
    /// <param name="index">The index of the element.</param>
    /// <returns>
    /// A string representing the element prefix, e.g. "CollectionProperty[0]".
    /// </returns>
    private static string BuildElementPrefix(
        string propertyPrefix, int index) => $"{propertyPrefix}[{index}]";

    /// <summary>
    /// Builds a key for a property of an element at a given index.
    /// </summary>
    /// <param name="elementPrefix">The prefix for the element (e.g. "CollectionProperty[0]").</param>
    /// <param name="property">The property of the element.</param>
    /// <returns>
    /// A string representing the full key, e.g. "CollectionProperty[0].PropertyName".
    /// </returns>
    private static string BuildElementPropertyKey(
        string elementPrefix, PropertyInfo property) => $"{elementPrefix}.{property.Name}";

    /// <summary>
    /// Checks whether the value provider has a non-empty value for the given key.
    /// </summary>
    /// <param name="provider">The value provider to query.</param>
    /// <param name="key">The key to look up.</param>
    /// <returns>
    /// <c>true</c> if the value provider contains a non-empty value for the key; otherwise <c>false</c>.
    /// </returns>
    private static bool HasValue(
        IValueProvider provider, string key)
    {
        ValueProviderResult result = provider.GetValue(key);
        return provider.GetValue(key) != ValueProviderResult.None &&
            !string.IsNullOrEmpty(result.FirstValue);
    }
}
