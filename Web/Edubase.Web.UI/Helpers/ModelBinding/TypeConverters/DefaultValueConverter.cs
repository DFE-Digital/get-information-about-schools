using System;
using System.ComponentModel;

namespace Edubase.Web.UI.Helpers.ModelBinding.TypeConverters;

/// <summary>
/// Provides a default implementation of <see cref="IValueConverter"/> for converting string values
/// into strongly typed .NET objects. This converter supports primitive types, enums, and types with
/// registered <see cref="TypeConverter"/> implementations.
/// </summary>
public class DefaultValueConverter : IValueConverter
{
    /// <summary>
    /// Converts a string representation of a value into an object of the specified target type.
    /// </summary>
    /// <param name="value">
    /// The input string to convert. Typically sourced from query strings, form fields, or route parameters.
    /// </param>
    /// <param name="targetType">
    /// The type to convert the input string to. This may be a primitive type, enum, nullable type,
    /// or any type supported by a registered <see cref="TypeConverter"/>.
    /// </param>
    /// <returns>
    /// An object of the specified target type, or <c>null</c> if the input is null or whitespace.
    /// </returns>
    /// <exception cref="FormatException">
    /// Thrown when a comma-separated value is provided for a single primitive type.
    /// </exception>
    /// <exception cref="InvalidCastException">
    /// Thrown when the target type cannot be converted from a string.
    /// </exception>
    public object Convert(string value, Type targetType)
    {
        // Return null if the input is null, empty, or whitespace
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        // Unwrap nullable types to get the underlying non-nullable type
        Type underlyingType =
            Nullable.GetUnderlyingType(
                nullableType: targetType) ?? targetType;

        // Reject comma-separated values for single primitive types (e.g., "1,2" for int)
        if (value.Contains(',') &&
            (underlyingType.IsPrimitive || underlyingType == typeof(decimal)))
        {
            throw new FormatException(
                $"Cannot bind comma-separated value '{value}' to single type {targetType.Name}");
        }

        // Handle enum conversion with case-insensitive parsing
        if (underlyingType.IsEnum)
        {
            return Enum.Parse(underlyingType, value, ignoreCase: true);
        }

        // Use TypeDescriptor to get a converter for the target type
        TypeConverter converter =
            TypeDescriptor.GetConverter(type: underlyingType);

        // If the converter supports conversion from string, perform the conversion
        if (converter.CanConvertFrom(sourceType: typeof(string)))
        {
            return converter.ConvertFromInvariantString(value);
        }

        // If conversion is not supported, throw an exception
        throw new InvalidCastException(
            $"Cannot convert from string to {targetType.Name}");
    }
}
