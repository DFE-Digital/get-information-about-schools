using System;
using System.Threading.Tasks;
using System.Reflection;
using Edubase.Web.UI.App_Start;
using Edubase.Web.UI.Helpers.ModelBinding;
using Edubase.Web.UI.Helpers.ModelBinding.BindingHandler;
using Edubase.Web.UIUnitTests.Helpers.ModelBinding.TestDoubles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Xunit;

namespace Edubase.Web.UIUnitTests.Helpers.ModelBinding;

public class DefaultModelBinderProviderTests
{
    [Fact]
    public async Task BindModelAsync_UsesCurrentRequestScope_AndDisposesScopedDependencies()
    {
        // Arrange
        ServiceCollection services = new();
        services.AddMvcServices();
        services.RemoveAll<IPropertyBinderHandler>();
        services.AddScoped<IPropertyBinderHandler>(_ =>
        {
            var handler = new Mock<IPropertyBinderHandler>();
            handler.As<IDisposable>();
            handler.Setup(x => x.HandleAsync(It.IsAny<ModelBindingContext>(), It.IsAny<object>(), It.IsAny<PropertyInfo>()))
                .ReturnsAsync(true);
            return handler.Object;
        });

        using var rootProvider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
        DefaultModelBinderProvider provider = new();
        var providerContext = new ModelBinderProviderContextTestDoubles(typeof(RequestModel));
        IModelBinder binder = provider.GetBinder(providerContext);
        IPropertyBinderHandler previousHandler = null;

        // Reuse the cached binder across requests.
        for (var request = 0; request < 2; request++)
        {
            Mock<IPropertyBinderHandler> handler;
            using (var scope = rootProvider.CreateScope())
            {
                var requestHandler = scope.ServiceProvider.GetRequiredService<IPropertyBinderHandler>();
                Assert.NotSame(previousHandler, requestHandler);
                previousHandler = requestHandler;
                handler = Mock.Get(requestHandler);
                var httpContext = new DefaultHttpContext { RequestServices = scope.ServiceProvider };
                var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
                var bindingContext = DefaultModelBindingContext.CreateBindingContext(actionContext,
                    new CompositeValueProvider(), providerContext.Metadata, null, "model");

                // Act
                await binder.BindModelAsync(bindingContext);

                // Assert
                Assert.IsType<RequestModel>(bindingContext.Result.Model);
                handler.Verify(x => x.HandleAsync(bindingContext, bindingContext.Result.Model, It.IsAny<PropertyInfo>()), Times.Once);
                handler.As<IDisposable>().Verify(x => x.Dispose(), Times.Never);
            }
            handler.As<IDisposable>().Verify(x => x.Dispose(), Times.Once);
        }
    }

    [Fact]
    public void GetBinder_BodyBinding_ReturnsNull()
    {
        // Arrange
        DefaultModelBinderProvider provider = new();
        var context = new ModelBinderProviderContextTestDoubles(typeof(string));
        context.BindingInfo.BindingSource = BindingSource.Body;

        // Act & Assert
        Assert.Null(provider.GetBinder(context));
    }

    public class RequestModel
    {
        public string Value { get; set; }
    }
}
