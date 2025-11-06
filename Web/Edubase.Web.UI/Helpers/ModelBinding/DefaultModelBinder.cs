using System;
using System.Reflection;
using System.Threading.Tasks;
using Edubase.Web.UI.Helpers.ModelBinding;
using Edubase.Web.UI.Helpers.ModelBinding.Extensions;
using Edubase.Web.UI.Helpers.ModelBinding.Factories;
using Edubase.Web.UI.Helpers.ModelBinding.TypeConverters;
using Microsoft.AspNetCore.Mvc.ModelBinding;


/// <summary>
/// Custom model binder that supports binding of simple types, complex objects, and collections.
/// Uses type resolution and conversion helpers to dynamically populate model properties.
/// </summary>
public class DefaultModelBinder : IModelBinder
{
    private readonly ITypeConverter _typeConverter;
    private readonly ITypeFactory _typeFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultModelBinder"/> class.
    /// </summary>
    public DefaultModelBinder(
        ITypeConverter typeConverter,
        ITypeFactory typeFactory)
    {
        _typeConverter = typeConverter;
        _typeFactory = typeFactory;
    }

    /// <summary>
    /// Binds the model from the value provider context.
    /// Handles simple types directly and recursively binds complex types and collections.
    /// </summary>
    /// <param name="bindingContext">The model binding context.</param>
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        Type modelType = bindingContext.ModelType.ResolveConcreteType();

        // Handle simple types directly
        if (modelType.IsSimpleType())
        {
            string value =
                bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue;

            object converted =
                _typeConverter.Convert(value, modelType);

            bindingContext.Result = ModelBindingResult.Success(converted);
            return;
        }

        // Create an instance of the complex model
        object modelInstance =
            _typeFactory.CreateInstance(modelType);

        PropertyInfo[] properties =
            modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo property in properties)
        {
            // Skip indexers and non-settable properties
            if (property.GetIndexParameters().Length > 0 ||
                !property.CanWrite ||
                property.GetSetMethod() == null)
            {
                continue;
            }

            bool propertySet =
                TryBindFromAliases(bindingContext, modelInstance, property);

            if (!propertySet && !property.PropertyType.IsSimpleType())
            {
                propertySet =
                    await TryBindComplexTypeAsync(bindingContext, modelInstance, property);
            }

            if (!propertySet)
            {
                TryBindFromPropertyName(bindingContext, modelInstance, property);
            }
        }

        bindingContext.Result = ModelBindingResult.Success(modelInstance);
    }

    /// <summary>
    /// Attempts to bind a property using its <see cref="BindAliasAttribute"/> aliases.
    /// </summary>
    private bool TryBindFromAliases(ModelBindingContext context, object model, PropertyInfo property)
    {
        foreach (var alias in property.GetCustomAttributes<BindAliasAttribute>(true))
        {
            ValueProviderResult valueResult =
                context.ValueProvider.GetValue(alias.Alias);

            if (valueResult == ValueProviderResult.None)
            {
                continue;
            }

            try
            {
                object converted =
                    _typeConverter.Convert(valueResult.FirstValue, property.PropertyType);

                property.SetValue(model, converted);
                return true;
            }
            catch
            {
                // Ignore failed conversions and continue
            }
        }

        return false;
    }

    /// <summary>
    /// Attempts to bind a complex property recursively using a nested model binder.
    /// </summary>
    private async Task<bool> TryBindComplexTypeAsync(ModelBindingContext context, object model, PropertyInfo property)
    {
        string propertyPrefix =
            string.IsNullOrEmpty(context.ModelName)
                ? property.Name
                : $"{context.ModelName}.{property.Name}";

        IModelMetadataProvider metadataProvider =
            context.ActionContext.HttpContext.RequestServices
                .GetService(typeof(IModelMetadataProvider)) as IModelMetadataProvider;

        ModelBindingContext nestedContext =
            DefaultModelBindingContext.CreateBindingContext(
                context.ActionContext,
                context.ValueProvider,
                metadataProvider.GetMetadataForType(property.PropertyType),
                bindingInfo: null,
                modelName: propertyPrefix);

        await new DefaultModelBinder(_typeConverter, _typeFactory).BindModelAsync(nestedContext);

        if (nestedContext.Result.IsModelSet)
        {
            property.SetValue(model, nestedContext.Result.Model);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Attempts to bind a property using its direct name in the value provider.
    /// </summary>
    private void TryBindFromPropertyName(ModelBindingContext context, object model, PropertyInfo property)
    {
        string propertyPrefix =
            string.IsNullOrEmpty(context.ModelName)
                ? property.Name
                : $"{context.ModelName}.{property.Name}";

        ValueProviderResult valueResult =
            context.ValueProvider.GetValue(propertyPrefix);

        if (valueResult == ValueProviderResult.None)
        {
            return;
        }

        object converted =
            _typeConverter.Convert(valueResult.FirstValue, property.PropertyType);

        property.SetValue(model, converted);
    }
}
