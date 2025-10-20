using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Edubase.Web.UI.Helpers.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public class BindAliasModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        var modelType = context.ModelType;
        var modelInstance = Activator.CreateInstance(modelType);

        foreach (var property in modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var aliases = property.GetCustomAttributes<BindAliasAttribute>().Select(a => a.Alias);

            foreach (var alias in aliases)
            {
                var valueResult = context.ValueProvider.GetValue(alias);
                if (valueResult != ValueProviderResult.None)
                {
                    var value = valueResult.FirstValue;
                    var converted = Convert.ChangeType(value, property.PropertyType);
                    property.SetValue(modelInstance, converted);
                }
            }

            // fallback to normal property name
            var fallbackValue = context.ValueProvider.GetValue(property.Name);
            if (fallbackValue != ValueProviderResult.None)
            {
                var value = fallbackValue.FirstValue;
                var converted = Convert.ChangeType(value, property.PropertyType);
                property.SetValue(modelInstance, converted);
            }
        }

        context.Result = ModelBindingResult.Success(modelInstance);
        return Task.CompletedTask;
    }
}
