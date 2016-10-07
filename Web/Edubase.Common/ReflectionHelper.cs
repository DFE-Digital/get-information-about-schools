using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Edubase.Common
{
    public class ReflectionHelper
    {
        public static string GetProperty(object item, string propertyName)
        {
            Type type = null;
            if (item is Type) type = (Type)item;
            else type = item.GetType();
            var propInfo = type.GetProperty(propertyName);
            var val = propInfo?.GetValue(item, null);
            return val?.ToString() ?? string.Empty;
        }

        public static void SetProperty(object item, string propertyName, object value)
        {
            Type type = null;
            if (item is Type) type = (Type)item;
            else type = item.GetType();
            var propInfo = type.GetProperty(propertyName);
            propInfo.SetValue(item, value, null);
        }

        public static List<string> GetProperties(object item)
        {
            Type type = null;
            if (item is Type) type = (Type)item;
            else type = item.GetType();
            PropertyInfo[] props = type.GetProperties();
            return props.Select(p => p.Name).ToList();
        }
    }
}
