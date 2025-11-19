using System;
using System.Reflection;
using System.Threading.Tasks;
using Edubase.Web.UI.Helpers.ModelBinding.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Edubase.Web.UI.Helpers.ModelBinding.BindingHandler.Handlers;

/// <summary>
/// Handler that binds complex types recursively using nested model binders.
/// </summary>
public sealed class ComplexTypeBinderHandler(
    IModelMetadataProvider metadataProvider) : PropertyBinderHandler
{
    public override async Task<bool> HandleAsync(
        ModelBindingContext context, object model, PropertyInfo property)
    {
        // Skip indexers and non-settable properties
        if (property.GetIndexParameters().Length > 0 ||
            !property.CanWrite ||
            property.GetSetMethod() == null)
        {
            return await base.HandleAsync(context, model, property);
        }

        // Skip simple types (handled by other handlers)
        if (property.PropertyType.IsSimpleType())
        {
            return await base.HandleAsync(context, model, property);
        }

        string propertyPrefix =
            string.IsNullOrEmpty(context.ModelName)
                ? property.Name
                : $"{context.ModelName}.{property.Name}";

        ModelBindingContext nestedContext =
            DefaultModelBindingContext.CreateBindingContext(
                context.ActionContext,
                context.ValueProvider,
                metadataProvider.GetMetadataForType(property.PropertyType),
                bindingInfo: null,
                modelName: propertyPrefix);

        // Try and invoke the nested binder.
        if (context.ActionContext.HttpContext.RequestServices.GetService(typeof(IModelBinderFactory))
            is not IModelBinderFactory modelBinderFactory)
        {
            throw new InvalidOperationException(
                "IModelBinderFactory service is not available.");
        }

        IModelBinder modelBinder =
            modelBinderFactory.CreateBinder(
                new ModelBinderFactoryContext
                {
                    BindingInfo = null,
                    Metadata = metadataProvider.GetMetadataForType(property.PropertyType),
                    CacheToken = property.PropertyType
                });

        await modelBinder.BindModelAsync(nestedContext);

        if (nestedContext.Result.IsModelSet &&
            nestedContext.Result.Model != null &&
            property.PropertyType.IsAssignableFrom(nestedContext.Result.Model.GetType()))
        {
            property.SetValue(model, nestedContext.Result.Model);
            return true;
        }

        return await base.HandleAsync(context, model, property);
    }
}
