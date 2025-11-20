using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Edubase.Web.UI.Helpers.ModelBinding.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Edubase.Web.UI.Helpers.ModelBinding.BindingHandler;

/// <summary>
/// A model binder handler that specializes in binding <b>simple types</b> such as
/// <see cref="string"/>, primitive types (<see cref="int"/>, <see cref="bool"/>, etc.),
/// <see cref="Enum"/> values, and common framework types like <see cref="DateTime"/>,
/// <see cref="Guid"/>, and <see cref="TimeSpan"/>.
/// </summary>
/// <remarks>
/// <para>
/// This handler is part of a chain of responsibility for property binding.
/// It attempts to bind simple types directly from the <see cref="ModelBindingContext.ValueProvider"/>.
/// If binding succeeds, the property or model is set and the handler short-circuits.
/// If binding fails, the request is delegated to the next handler in the chain.
/// </para>
/// <para>
/// Conversion logic is centralized in a strategy dictionary, allowing custom parsing
/// for specific types (e.g., <see cref="Guid"/> or <see cref="DateTime"/>).
/// Enums are handled explicitly via <see cref="Enum.Parse(Type,string,bool)"/>.
/// </para>
/// </remarks>
public class SimpleTypeBinderHandler : IPropertyBinderHandler
{
    private IPropertyBinderHandler _next;

    /// <summary>
    /// Sets the next handler in the chain of responsibility.
    /// </summary>
    /// <param name="next">The next handler to invoke if this handler cannot bind.</param>
    /// <returns>The handler that was set as next, enabling fluent chaining.</returns>
    public IPropertyBinderHandler SetNext(IPropertyBinderHandler next)
    {
        _next = next;
        return next;
    }

    /// <summary>
    /// Attempts to bind a simple type property on a complex model.
    /// </summary>
    /// <param name="context">
    /// The current <see cref="ModelBindingContext"/> containing metadata, value providers,
    /// and binding state.
    /// </param>
    /// <param name="model">The parent model instance being bound.</param>
    /// <param name="property">The <see cref="PropertyInfo"/> representing the property to bind.</param>
    /// <returns>
    /// A task that resolves to <c>true</c> if the property was successfully bound,
    /// or <c>false</c> if binding failed and should be passed to the next handler.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method:
    /// <list type="number">
    ///   <item>Checks if the property type is simple.</item>
    ///   <item>Retrieves the raw value from the <see cref="ModelBindingContext.ValueProvider"/>.</item>
    ///   <item>Converts the raw string into the target type using <see cref="ConvertValue"/>.</item>
    ///   <item>Sets the property value if conversion succeeds.</item>
    /// </list>
    /// </para>
    /// <para>
    /// If conversion fails, the binding result is marked as failed and the chain continues.
    /// </para>
    /// </remarks>
    public async Task<bool> HandleAsync(
        ModelBindingContext context, object model, PropertyInfo property)
    {
        if (property.PropertyType.IsSimpleType())
        {
            ValueProviderResult valueResult =
                context.ValueProvider.GetValue(property.Name);

            if (valueResult != ValueProviderResult.None)
            {
                try
                {
                    object convertedValue =
                        ConvertValue(valueResult.FirstValue, property.PropertyType);

                    property.SetValue(model, convertedValue);
                    return true;
                }
                catch
                {
                    context.Result = ModelBindingResult.Failed();
                }
            }
        }

        return _next != null &&
            await _next.HandleAsync(context, model, property);
    }

    /// <summary>
    /// Binds a top-level simple model directly (e.g. string, int, DateTime, enum).
    /// </summary>
    /// <param name="context">The current model binding context.</param>
    /// <returns>
    /// A task that resolves to <c>true</c> if the model was successfully bound,
    /// or <c>false</c> if the type is not simple.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method is used when the action parameter itself is a simple type,
    /// rather than a property on a complex object.
    /// </para>
    /// </remarks>
    public async Task<bool> BindModelAsync(ModelBindingContext context)
    {
        Type modelType = context.ModelType;

        if (!modelType.IsSimpleType())
        {
            return false;
        }

        ValueProviderResult valueResult =
            context.ValueProvider.GetValue(context.ModelName);

        if (valueResult == ValueProviderResult.None)
        {
            context.Result = ModelBindingResult.Failed();
            return true;
        }

        try
        {
            object convertedValue =
                ConvertValue(valueResult.FirstValue, modelType);

            context.Result = ModelBindingResult.Success(convertedValue);
            return true;
        }
        catch
        {
            context.Result = ModelBindingResult.Failed();
            return true;
        }
    }

    /// <summary>
    /// Centralized conversion logic for simple types.
    /// </summary>
    /// <param name="rawValue">The raw string value from the request.</param>
    /// <param name="targetType">The target type to convert to.</param>
    /// <returns>The converted object instance.</returns>
    /// <exception cref="FormatException">Thrown if parsing fails (e.g., invalid Guid or DateTime).</exception>
    /// <exception cref="ArgumentException">Thrown if the value cannot be parsed into an enum.</exception>
    /// <remarks>
    /// <para>
    /// Conversion order:
    /// <list type="number">
    ///   <item>Unwrap nullable types to their underlying type.</item>
    ///   <item>If the type is an enum, parse via <see cref="Enum.Parse(Type,string,bool)"/>.</item>
    ///   <item>If the type exists in the <see cref="Converters"/> dictionary, use the custom strategy.</item>
    ///   <item>Otherwise, fall back to <see cref="Convert.ChangeType(object,Type)"/>.</item>
    /// </list>
    /// </para>
    /// </remarks>
    private static object ConvertValue(string rawValue, Type targetType)
    {
        targetType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        return targetType.IsEnum
            ? Enum.Parse(targetType, rawValue, ignoreCase: true)
                : Converters.TryGetValue(targetType, out var converter) ?
                converter(rawValue) : Convert.ChangeType(rawValue, targetType);
    }

    /// <summary>
    /// A dictionary of type-specific conversion strategies.
    /// <para>
    /// This allows custom parsing for types that require more than
    /// <see cref="Convert.ChangeType(object,Type)"/>.
    /// </para>
    /// <para>
    /// For example, <see cref="Guid"/> and <see cref="DateTime"/> values are parsed
    /// using their dedicated parsing methods to ensure correct conversion.
    /// </para>
    /// </summary>
    private static readonly Dictionary<
        Type, Func<string, object>> Converters =
            new()
            {
                { typeof(string), str => str },
                { typeof(Guid), str => Guid.Parse(str) },
                { typeof(DateTime), str => DateTime.Parse(str) },
                { typeof(DateTimeOffset), str => DateTimeOffset.Parse(str) },
                { typeof(TimeSpan), str => TimeSpan.Parse(str) }
            };
}
