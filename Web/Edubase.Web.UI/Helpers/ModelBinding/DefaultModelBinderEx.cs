using System;
using System.Reflection;
using System.Threading.Tasks;
using Edubase.Web.UI.Helpers.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public class DefaultModelBinderEx : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
            throw new ArgumentNullException(nameof(bindingContext));

        var modelType = bindingContext.ModelType;
        var modelInstance = Activator.CreateInstance(modelType);

        foreach (var property in modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            // Check for alias attributes
            var aliasAttributes = property.GetCustomAttributes<BindAliasAttribute>();
            foreach (var alias in aliasAttributes)
            {
                var valueResult = bindingContext.ValueProvider.GetValue(alias.Alias);
                if (valueResult != ValueProviderResult.None)
                {
                    var value = valueResult.FirstValue;
                    var converted = Convert.ChangeType(value, property.PropertyType);
                    property.SetValue(modelInstance, converted);
                }
            }

            // Fallback to default property name
            var defaultValue = bindingContext.ValueProvider.GetValue(property.Name);
            if (defaultValue != ValueProviderResult.None)
            {
                var value = defaultValue.FirstValue;
                var converted = Convert.ChangeType(value, property.PropertyType);
                property.SetValue(modelInstance, converted);
            }
        }

        bindingContext.Result = ModelBindingResult.Success(modelInstance);
        return Task.CompletedTask;
    }
}
