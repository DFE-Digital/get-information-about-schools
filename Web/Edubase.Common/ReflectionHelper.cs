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

        public static List<ChangeDescriptor> DetectChanges(object obj, object obj2, params Type[] types)
        {
            return DetectChanges(obj, obj2, null, types);
        }
        
        public static List<ChangeDescriptor> DetectChanges(object newModel, object oldModel, string prefixer, params Type[] types)
        {
            if (!string.IsNullOrWhiteSpace(prefixer)) prefixer += ".";
            var retVal = new List<ChangeDescriptor>();
            
            var props = GetProperties(newModel);
            foreach (var prop in props)
            {
                var p = newModel.GetType().GetProperty(prop);
                var p2 = oldModel.GetType().GetProperty(prop);
                if (types.Any(x => x == p.PropertyType))
                {
                    retVal.AddRange(DetectChanges(p.GetValue(newModel), p2.GetValue(oldModel), p.Name, types));
                }
                else if(p.PropertyType.IsValueType)
                {
                    var v1 = (ValueType) p.GetValue(newModel);
                    var v2 = (ValueType) p2.GetValue(oldModel);
                    if ((v1 == null && v2 != null) 
                        || (v1 != null && v2 == null) 
                        || (v1 != null && !v1.Equals(v2) 
                        || (v2 != null && !v2.Equals(v1))))
                    {
                        retVal.Add(new ChangeDescriptor(prefixer + p.Name, v1, v2));
                    }
                }
                else if (p.PropertyType == typeof(string))
                {
                    var v1 = p.GetValue(newModel)?.ToString();
                    var v2 = p2.GetValue(oldModel)?.ToString();
                    if (v1 != v2)
                    {
                        retVal.Add(new ChangeDescriptor(prefixer + p.Name, v1, v2));
                    }
                }
            }

            return retVal;
        }
    }
}
