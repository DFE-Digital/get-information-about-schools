using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace Edubase.Common.Reflection
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

        public static object GetPropertyValue(object item, string propertyName)
        {
            Type type = null;
            if (item is Type) type = (Type)item;
            else type = item.GetType();
            var propInfo = type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            var val = propInfo?.GetValue(item, null);
            return val;
        }

        public static T GetPropertyValue<T>(object item, string propertyName)
        {
            return (T)(GetPropertyValue(item, propertyName) ?? default(T));
        }

        public static PropInfo GetPropertyInfo(object item, string propertyName)
        {
            Type type = null;
            if (item is Type) type = (Type)item;
            else type = item.GetType();
            var propInfo = type.GetProperty(propertyName);
            var val = propInfo?.GetValue(item, null);
            return new PropInfo { Name = propertyName, Type = propInfo?.PropertyType, Value = val };
        }



        public static void SetProperty(object item, string propertyName, object value)
        {
            Type type = null;
            if (item is Type) type = (Type)item;
            else type = item.GetType();
            var propInfo = type.GetProperty(propertyName);
            propInfo.SetValue(item, value, null);
        }

        public static List<string> GetProperties(object item, Type ignoreAttributeType = null, bool writeableOnly = false)
        {
            Type type = null;
            if (item is Type) type = (Type)item;
            else type = item.GetType();
            var props = type.GetProperties();
            return props.Where(x => (ignoreAttributeType == null || !x.CustomAttributes.Any(a => a.AttributeType == ignoreAttributeType))
                && (writeableOnly == false || x.CanWrite))
                .Select(p => p.Name).ToList();
        }

        




        public static List<ChangeDescriptor> DetectChanges(object newModel, object oldModel, params Type[] types)
        {
            return DetectChanges(newModel, oldModel, null, types);
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

                var propertyName = p.Name;
                var displayName = p.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;

                if((p.GetCustomAttribute<IgnoreChangesAttribute>() ?? p2.GetCustomAttribute<IgnoreChangesAttribute>()) != null)
                {
                    continue;
                }

                if (types.Any(x => x == p.PropertyType))
                {
                    retVal.AddRange(DetectChanges(p.GetValue(newModel), p2.GetValue(oldModel), propertyName, types));
                }
                else if (p.PropertyType.IsGenericType &&
                         p.PropertyType.GetGenericTypeDefinition() == typeof(List<>) &&
                         types.Any(x => x == p.PropertyType.GetGenericArguments().Single())
                        )
                {
                    // test for lists containing key property types as well.
                    IList theNewList = p.GetValue(newModel, null) as IList;
                    IList theOldList = p2.GetValue(oldModel, null) as IList;

                    for (int i = 0; i < theNewList.Count; i++)
                    {
                        if (theNewList.Count > i && theOldList.Count > i)
                        {
                            retVal.AddRange(DetectChanges(theNewList[i], theOldList[i], propertyName, types));
                        }
                    }

                    if (theNewList.Count != theOldList.Count)
                    {
                        // test the count is the same
                        var v1 = theNewList.Count;
                        var v2 = theOldList.Count;
                        retVal.Add(new ChangeDescriptor(prefixer + propertyName, $"{propertyName} items", v1, v2));
                    }
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
                        retVal.Add(new ChangeDescriptor(prefixer + propertyName, displayName, v1, v2));
                    }
                }
                else if (p.PropertyType == typeof(string))
                {
                    var v1 = p.GetValue(newModel)?.ToString();
                    var v2 = p2.GetValue(oldModel)?.ToString();
                    if (v1 != v2)
                    {
                        retVal.Add(new ChangeDescriptor(prefixer + propertyName, displayName, v1, v2));
                    }
                }
            }

            return retVal;
        }
    }
}
