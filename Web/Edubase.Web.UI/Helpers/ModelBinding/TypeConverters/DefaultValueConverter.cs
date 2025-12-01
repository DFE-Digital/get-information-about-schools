using System;
using System.ComponentModel;
using System.Globalization;

namespace Edubase.Web.UI.Helpers.ModelBinding.TypeConverters;

/// <summary>
/// Provides a default implementation of <see cref="IValueConverter"/> for converting string values
/// into strongly typed .NET objects. This converter supports primitive types, enums, and types with
/// registered <see cref="TypeConverter"/> implementations.
/// </summary>
public class DefaultValueConverter : IValueConverter
{
    private const string UkDateTimeFormat = "dd/MM/yyyy HH:mm:ss";
    private const string EnGbCultureName = "en-GB";

    /// <summary>
    /// Converts a string value into the specified target type, handling common cases
    /// such as nullables, enums, DateTime, and primitive types.
    /// </summary>
    /// <param name="value">
    /// The string input to convert. If null, empty, or whitespace, the method returns null.
    /// </param>
    /// <param name="targetType">
    /// The type to convert the string into. Nullable types are automatically unwrapped.
    /// </param>
    /// <returns>
    /// An object of the target type representing the converted value, or null if the input
    /// is empty. Throws exceptions if conversion is invalid.
    /// </returns>
    /// <exception cref="FormatException">
    /// Thrown when attempting to bind a comma-separated string to a single primitive or decimal type.
    /// </exception>
    /// <exception cref="InvalidCastException">
    /// Thrown when no suitable type converter exists to convert from string to the target type.
    /// </exception>
    /// <exception cref="Exception">
    /// May be thrown by underlying conversion logic (e.g., DateTime parsing failures).
    /// </exception>
    public object Convert(string value, Type targetType)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        // Unwrap nullable types
        Type underlyingType =
            Nullable.GetUnderlyingType(targetType) ?? targetType;

        // Reject comma-separated values for single primitive types
        if (value.Contains(',') &&
            (underlyingType.IsPrimitive ||
            underlyingType == typeof(decimal)))
        {
            throw new FormatException(
                $"Cannot bind comma-separated value '{value}' to single type {targetType.Name}");
        }

        // Handle enums
        if (underlyingType.IsEnum)
        {
            return Enum.Parse(underlyingType, value, ignoreCase: true);
        }

        // Special handling for DateTime
        if (underlyingType == typeof(DateTime))
        {
            // Try UK format first
            if (DateTime.TryParseExact(value,
                UkDateTimeFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsed))
            {
                return parsed;
            }

            // Try en-GB culture
            if (DateTime.TryParse(value,
                CultureInfo.GetCultureInfo(EnGbCultureName),
                DateTimeStyles.None,
                out parsed))
            {
                return parsed;
            }

            // Fallback to invariant
            return DateTime.Parse(value, CultureInfo.InvariantCulture);
        }

        // Use TypeDescriptor converter for other types
        TypeConverter converter =
            TypeDescriptor.GetConverter(underlyingType);

        return converter.CanConvertFrom(typeof(string))
            ? converter.ConvertFromInvariantString(value)
            : throw new InvalidCastException(
                $"Cannot convert from string to {targetType.Name}");
    }
}
