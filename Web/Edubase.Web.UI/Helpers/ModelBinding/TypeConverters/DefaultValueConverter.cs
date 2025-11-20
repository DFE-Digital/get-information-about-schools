using System;
using System.ComponentModel;
using System.Globalization;

namespace Edubase.Web.UI.Helpers.ModelBinding.TypeConverters
{
    /// <summary>
    /// Provides a default implementation of <see cref="IValueConverter"/> for converting string values
    /// into strongly typed .NET objects. This converter supports primitive types, enums, and types with
    /// registered <see cref="TypeConverter"/> implementations.
    /// </summary>
    public class DefaultValueConverter : IValueConverter
    {
        public object Convert(string value, Type targetType)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            // Unwrap nullable types
            Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            // Reject comma-separated values for single primitive types
            if (value.Contains(',') &&
                (underlyingType.IsPrimitive || underlyingType == typeof(decimal)))
            {
                throw new FormatException(
                    $"Cannot bind comma-separated value '{value}' to single type {targetType.Name}");
            }

            // Handle enums
            if (underlyingType.IsEnum)
            {
                return Enum.Parse(underlyingType, value, ignoreCase: true);
            }

            // ✅ Special handling for DateTime
            if (underlyingType == typeof(DateTime))
            {
                // Try UK format first
                if (DateTime.TryParseExact(value,
                    "dd/MM/yyyy HH:mm:ss",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var parsed))
                {
                    return parsed;
                }

                // Try en-GB culture
                if (DateTime.TryParse(value,
                    CultureInfo.GetCultureInfo("en-GB"),
                    DateTimeStyles.None,
                    out parsed))
                {
                    return parsed;
                }

                // Fallback to invariant
                return DateTime.Parse(value, CultureInfo.InvariantCulture);
            }

            // Use TypeDescriptor converter for other types
            TypeConverter converter = TypeDescriptor.GetConverter(underlyingType);
            if (converter.CanConvertFrom(typeof(string)))
            {
                return converter.ConvertFromInvariantString(value);
            }

            throw new InvalidCastException(
                $"Cannot convert from string to {targetType.Name}");
        }
    }
}
