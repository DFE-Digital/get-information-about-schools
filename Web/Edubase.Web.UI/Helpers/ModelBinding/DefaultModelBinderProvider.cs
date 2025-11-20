using System;
using System.Collections.Generic;
using Edubase.Web.UI.Helpers.ModelBinding.BindingHandler;
using Edubase.Web.UI.Helpers.ModelBinding.Factories;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace Edubase.Web.UI.Helpers.ModelBinding;

/// <summary>
/// Provides a custom model binder to the ASP.NET Core model binding system.
/// This binder is responsible for resolving and supplying an instance of <see cref="DefaultModelBinder"/>
/// using scoped services for type conversion, object creation, and property binder handlers.
/// </summary>
public class DefaultModelBinderProvider : IModelBinderProvider
{
    private readonly IServiceProvider _services;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultModelBinderProvider"/> class.
    /// </summary>
    /// <param name="services">
    /// The root <see cref="IServiceProvider"/> used to resolve scoped dependencies.
    /// </param>
    public DefaultModelBinderProvider(IServiceProvider services)
    {
        _services = services;
    }

    /// <summary>
    /// Returns an instance of <see cref="IModelBinder"/> for the given binding context.
    /// </summary>
    /// <param name="context">The model binder provider context.</param>
    /// <returns>
    /// A configured <see cref="DefaultModelBinder"/> instance with scoped dependencies.
    /// </returns>
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        // Resolve a scope factory from the root service provider
        IServiceScopeFactory scopeFactory =
            _services.GetRequiredService<IServiceScopeFactory>();

        // Create a new service scope for resolving scoped services
        IServiceScope scope = scopeFactory.CreateScope();

        // Resolve the required services from the scoped provider
        IEnumerable<IPropertyBinderHandler> handlers =
            scope.ServiceProvider
                .GetRequiredService<IEnumerable<IPropertyBinderHandler>>();

        ITypeFactory factory =
            scope.ServiceProvider
                .GetRequiredService<ITypeFactory>();

        // Construct the model binder with injected handlers and factory
        DefaultModelBinder binder = new(handlers, factory);

        return binder;
    }
}
