using System;
using System.Collections.Generic;
using System.Reflection;
using Edubase.Web.UI.Helpers.ModelBinding.Extensions;
using Edubase.Web.UI.Helpers.ModelBinding.Factories;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace Edubase.Web.UI.Helpers.ModelBinding.BindingHandler.Handlers.Extensions;

/// <summary>
/// Provides extension methods for <see cref="PropertyInfo"/> to support
/// custom model binding scenarios such as determining property suitability,
/// resolving nested model instances, and retrieving binding metadata.
/// </summary>
public static class PropertyInfoExtensions
{
    /// <summary>
    /// Determines whether a property should be skipped by the complex type binder.
    /// </summary>
    /// <param name="property">The property being evaluated.</param>
    /// <returns>
    /// <c>true</c> if the property should not be processed by the complex binder;
    /// otherwise <c>false</c>.
    /// </returns>
    /// <remarks>
    /// A property is skipped if it is:
    /// <list type="bullet">
    /// <item><description>An indexer</description></item>
    /// <item><description>Non‑writable</description></item>
    /// <item><description>A simple type</description></item>
    /// <item><description>A <see cref="List{T}"/></description></item>
    /// <item><description>An array of simple types</description></item>
    /// </list>
    /// </remarks>
    public static bool ShouldSkipProperty(this PropertyInfo property) =>
        IsIndexer(property) ||
        IsNonWritable(property) ||
        IsSimple(property) ||
        IsList(property) ||
        IsSimpleArray(property);

    /// <summary>
    /// Resolves or creates an instance of a nested complex type for model binding.
    /// </summary>
    /// <param name="property">The property representing the nested complex type.</param>
    /// <param name="nestedContext">The binding context containing the attempted binding result.</param>
    /// <returns>
    /// The resolved instance if binding succeeded or a new instance if the type
    /// has a public parameterless constructor; otherwise <c>null</c>.
    /// </returns>
    /// <remarks>
    /// This method is used when the default model binder cannot instantiate a type
    /// (e.g., due to constructor constraints). It safely falls back to manual instantiation
    /// only when the type is a concrete class with a public parameterless constructor.
    /// </remarks>
    public static object ResolveNestedModel(
        this PropertyInfo property, ModelBindingContext nestedContext)
    {
        if (nestedContext.Result.IsModelSet &&
            nestedContext.Result.Model != null &&
            property.PropertyType.IsAssignableFrom(nestedContext.Result.Model.GetType()))
        {
            return nestedContext.Result.Model;
        }

        Type type = property.PropertyType;

        ITypeFactory typeFactory =
            nestedContext.ActionContext.HttpContext.RequestServices
            .GetRequiredService<ITypeFactory>();

        return type.IsClass &&
               !type.IsAbstract &&
               !type.IsArray &&
               type.GetConstructor(Type.EmptyTypes) != null
            ? typeFactory.CreateInstance(type)
            : null;
    }

    /// <summary>
    /// Retrieves all <see cref="BindAliasAttribute"/> instances applied to the property.
    /// </summary>
    /// <param name="property">The property whose alias attributes should be retrieved.</param>
    /// <returns>
    /// An array of <see cref="BindAliasAttribute"/> instances, or an empty array if none exist.
    /// </returns>
    public static BindAliasAttribute[] GetBindAliases(this PropertyInfo property) =>
        [.. property.GetCustomAttributes<BindAliasAttribute>(true)];

    /// <summary>
    /// Determines whether the property is an indexer (i.e., defines index parameters).
    /// </summary>
    /// <param name="property">The property to inspect.</param>
    /// <returns>
    /// <c>true</c> if the property is an indexer; otherwise <c>false</c>.
    /// </returns>
    public static bool IsIndexer(this PropertyInfo property) =>
        property.GetIndexParameters().Length > 0;

    /// <summary>
    /// Determines whether the property cannot be written to.
    /// </summary>
    /// <param name="property">The property to inspect.</param>
    /// <returns>
    /// <c>true</c> if the property has no setter or is otherwise non‑writable; otherwise <c>false</c>.
    /// </returns>
    public static bool IsNonWritable(this PropertyInfo property) =>
        !property.CanWrite || property.GetSetMethod() == null;

    /// <summary>
    /// Determines whether the property's type is a simple type
    /// (e.g., <see cref="string"/>, <see cref="int"/>, <see cref="Enum"/>).
    /// </summary>
    /// <param name="property">The property whose type is being evaluated.</param>
    /// <returns>
    /// <c>true</c> if the property type is simple; otherwise <c>false</c>.
    /// </returns>
    public static bool IsSimple(this PropertyInfo property) =>
        property.PropertyType.IsSimpleType();

    /// <summary>
    /// Determines whether the property is a <see cref="List{T}"/>.
    /// </summary>
    /// <param name="property">The property to inspect.</param>
    /// <returns>
    /// <c>true</c> if the property is a generic <see cref="List{T}"/>; otherwise <c>false</c>.
    /// </returns>
    public static bool IsList(this PropertyInfo property) =>
        property.PropertyType.IsGenericType &&
        property.PropertyType.GetGenericTypeDefinition() == typeof(List<>);

    /// <summary>
    /// Determines whether the property is an array whose element type is simple.
    /// </summary>
    /// <param name="property">The property to inspect.</param>
    /// <returns>
    /// <c>true</c> if the property is an array of simple types; otherwise <c>false</c>.
    /// </returns>
    public static bool IsSimpleArray(this PropertyInfo property) =>
        property.PropertyType.IsArray &&
        property.PropertyType.GetElementType()?.IsSimpleType() == true;

    /// <summary>
    /// Determines whether the property is an array whose element type is complex.
    /// </summary>
    /// <param name="property">The property to inspect.</param>
    /// <returns>
    /// <c>true</c> if the property is an array of complex types; otherwise <c>false</c>.
    /// </returns>
    public static bool IsComplexArray(this PropertyInfo property) =>
        property.PropertyType.IsArray &&
        property.PropertyType.GetElementType()?.IsSimpleType() == false;
}
