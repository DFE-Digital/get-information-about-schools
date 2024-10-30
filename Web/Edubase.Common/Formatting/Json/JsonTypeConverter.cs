using AutoMapper;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Edubase.Common.Formatting.Json
{
    public class JsonTypeConverter<T> : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            => sourceType == typeof(T);

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            => destinationType == typeof(string);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null) return default(T);
            try
            {
                return JsonConvert.DeserializeObject<T>(value.ToString());
            }
            catch (JsonReaderException)
            {
                return default(T);
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            try
            {
                return JsonConvert.SerializeObject(value);
            }
            catch (JsonReaderException)
            {
                return string.Empty;
            }
        }
    }

    public class ToJsonTypeConverter<T> : ITypeConverter<T, string>
    {
        string ITypeConverter<T, string>.Convert(T source, string destination, ResolutionContext context)
        {
            if (source == null) return null;
            else return JsonConvert.SerializeObject(source);
        }
    }


    public class FromJsonTypeConverter<T> : ITypeConverter<string, T>
    {
        T ITypeConverter<string, T>.Convert(string source, T destination, ResolutionContext context)
        {
            if (string.IsNullOrWhiteSpace(source)) return default(T);

            try
            {
                return JsonConvert.DeserializeObject<T>(source);
            }
            catch (JsonReaderException)
            {
                return default(T);
            }
        }
    }

}
