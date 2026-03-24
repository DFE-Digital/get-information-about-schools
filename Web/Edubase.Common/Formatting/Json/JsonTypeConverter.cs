using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using Newtonsoft.Json;
using AutoMapper;

public class JsonTypeConverter<T> : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        => sourceType == typeof(string) || sourceType == typeof(T);

    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        => destinationType == typeof(string);

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value == null) return default(T);

        try
        {
            var str = value.ToString();
            if (string.IsNullOrWhiteSpace(str)) return default(T);

            return JsonConvert.DeserializeObject<T>(str);
        }
        catch (JsonException ex)
        {
            // Safe fallback: return default and optionally trace the issue
            Debug.WriteLine($"[JsonTypeConverter<{typeof(T).Name}>] Deserialization failed: {ex.Message}");
            return default(T);
        }
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        try
        {
            if (value == null) return null;
            return JsonConvert.SerializeObject(value);
        }
        catch (JsonException ex)
        {
            Debug.WriteLine($"[JsonTypeConverter<{typeof(T).Name}>] Serialization failed: {ex.Message}");
            return null;
        }
    }
}

public class ToJsonTypeConverter<T> : ITypeConverter<T, string>
{
    public string Convert(T source, string destination, ResolutionContext context)
    {
        if (source == null) return null;

        try
        {
            return JsonConvert.SerializeObject(source);
        }
        catch (JsonException ex)
        {
            Debug.WriteLine($"[ToJsonTypeConverter<{typeof(T).Name}>] Serialization failed: {ex.Message}");
            return null;
        }
    }
}

public class FromJsonTypeConverter<T> : ITypeConverter<string, T>
{
    public T Convert(string source, T destination, ResolutionContext context)
    {
        if (string.IsNullOrWhiteSpace(source)) return default(T);

        try
        {
            return JsonConvert.DeserializeObject<T>(source);
        }
        catch (JsonException ex)
        {
            Debug.WriteLine($"[FromJsonTypeConverter<{typeof(T).Name}>] Deserialization failed: {ex.Message}");
            return default(T);
        }
    }
}
