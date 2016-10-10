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

        public static List<string> DetectChanges(object obj, object obj2, params Type[] types)
        {
            return DetectChanges(obj, obj2, null, types);
        }

        public static List<string> DetectChanges(object obj, object obj2, string prefixer, params Type[] types)
        {
            if (!string.IsNullOrWhiteSpace(prefixer)) prefixer += ".";
            var retVal = new List<string>();
            
            var props = GetProperties(obj);
            foreach (var prop in props)
            {
                var p = obj.GetType().GetProperty(prop);
                var p2 = obj2.GetType().GetProperty(prop);
                if (types.Any(x => x == p.PropertyType))
                {
                    retVal.AddRange(DetectChanges(p.GetValue(obj), p2.GetValue(obj2), p.Name, types));
                }
                else if(p.PropertyType.IsValueType)
                {
                    var v1 = (ValueType) p.GetValue(obj);
                    var v2 = (ValueType) p2.GetValue(obj2);
                    if ((v1 == null && v2 != null) 
                        || (v1 != null && v2 == null) 
                        || (v1 != null && !v1.Equals(v2) 
                        || (v2 != null && !v2.Equals(v1))))
                    {
                        retVal.Add(prefixer + p.Name);
                    }
                }
                else if (p.PropertyType == typeof(string))
                {
                    var v1 = p.GetValue(obj)?.ToString();
                    var v2 = p2.GetValue(obj2)?.ToString();
                    if (v1 != v2)
                    {
                        retVal.Add(prefixer + p.Name);
                    }
                }
            }

            return retVal;
        }
    }
}
