using System;
using Edubase.Web.UI.Helpers.ModelBinding;
using Edubase.Web.UI.Helpers.ModelBinding.BindingHandler;
using Edubase.Web.UI.Helpers.ModelBinding.Factories;
using Edubase.Web.UIUnitTests.Helpers.ModelBinding.TestDoubles;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Edubase.Web.UIUnitTests.Helpers.ModelBinding;

public class DefaultModelBinderProviderTests
{
    [Fact]
    public void Constructor_StoresServiceProvider()
    {
        // Arrange
        IServiceProvider services =
            new ServiceCollection()
                    .BuildServiceProvider();

        // Act
        DefaultModelBinderProvider provider = new(services);

        // Assert
        Assert.NotNull(provider);
    }

    [Fact]
    public void GetBinder_ResolvesDefaultModelBinder_WithHandlersAndFactory()
    {
        // Arrange
        ServiceCollection services = new();
        services.AddScoped<IPropertyBinderHandler, PropertyBinderHandlerTestDoubles>();
        services.AddScoped<ITypeFactory, TypeFactoryTestDoubles>();
        IServiceProvider rootProvider = services.BuildServiceProvider();

        DefaultModelBinderProvider provider = new(rootProvider);

        ModelBinderProviderContext context =
            new ModelBinderProviderContextTestDoubles(typeof(string));

        // Act
        IModelBinder binder =
            provider.GetBinder(context);

        // Assert
        Assert.NotNull(binder);
        Assert.IsType<DefaultModelBinder>(binder);
    }

    [Fact]
    public void GetBinder_MissingScopeFactory_ThrowsInvalidOperationException()
    {
        // Arrange
        IServiceProvider rootProvider =
            new ServiceCollection()
                .BuildServiceProvider();

        DefaultModelBinderProvider provider = new(rootProvider);

        ModelBinderProviderContext context =
            new ModelBinderProviderContextTestDoubles(typeof(string));

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => provider.GetBinder(context));
    }

    [Fact]
    public void GetBinder_MissingHandlers_ThrowsInvalidOperationException()
    {
        // Arrange
        ServiceCollection services = new();
        services.AddScoped<ITypeFactory, TypeFactoryTestDoubles> ();
        IServiceProvider rootProvider = services.BuildServiceProvider();

        DefaultModelBinderProvider provider = new(rootProvider);

        ModelBinderProviderContext context =
            new ModelBinderProviderContextTestDoubles(typeof(string));

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => provider.GetBinder(context));
    }

    [Fact]
    public void GetBinder_MissingFactory_ThrowsInvalidOperationException()
    {
        // Arrange
        ServiceCollection services = new();
        services.AddScoped<IPropertyBinderHandler, PropertyBinderHandlerTestDoubles>();
        IServiceProvider rootProvider = services.BuildServiceProvider();

        DefaultModelBinderProvider provider = new(rootProvider);

        ModelBinderProviderContext context =
            new ModelBinderProviderContextTestDoubles(typeof(string));

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => provider.GetBinder(context));
    }
}
