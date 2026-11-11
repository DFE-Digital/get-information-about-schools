using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Edubase.Web.UI.Helpers.ModelBinding
{
    internal static class DefaultModelBinderEx
    {
        public static IReadOnlyDictionary<string, string[]> BuildAliasMap(ActionContext actionContext)
        {
            if (actionContext.ActionDescriptor is not ControllerActionDescriptor controllerAction)
            {
                return new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
            }

            var aliasLookup = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

            foreach (var parameter in controllerAction.MethodInfo.GetParameters())
            {
                if (!IsCandidateComplexType(parameter.ParameterType))
                {
                    continue;
                }

                foreach (var property in parameter.ParameterType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var aliases = property
                        .GetCustomAttributes(typeof(BindAliasAttribute), true)
                        .Cast<BindAliasAttribute>()
                        .Select(attribute => attribute.Alias)
                        .Where(alias => !string.IsNullOrWhiteSpace(alias));

                    foreach (var alias in aliases)
                    {
                        if (!aliasLookup.TryGetValue(property.Name, out var aliasSet))
                        {
                            aliasSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                            aliasLookup[property.Name] = aliasSet;
                        }

                        aliasSet.Add(alias!);
                    }
                }
            }

            return aliasLookup.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.ToArray(),
                StringComparer.OrdinalIgnoreCase);
        }

        private static bool IsCandidateComplexType(Type parameterType)
        {
            if (parameterType == typeof(string))
            {
                return false;
            }

            var underlyingNullableType = Nullable.GetUnderlyingType(parameterType);
            var typeToEvaluate = underlyingNullableType ?? parameterType;

            if (typeToEvaluate.IsPrimitive || typeToEvaluate.IsEnum)
            {
                return false;
            }

            return typeToEvaluate.IsClass;
        }
    }
}
