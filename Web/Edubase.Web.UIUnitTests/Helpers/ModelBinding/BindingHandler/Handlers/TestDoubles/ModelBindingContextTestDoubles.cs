using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Edubase.Web.UIUnitTests.Helpers.ModelBinding.BindingHandler.Handlers.TestDoubles;

/// <summary>
/// Provides reusable helpers for constructing <see cref="ModelBindingContext"/> instances
/// in unit tests. These helpers reduce boilerplate when simulating ASP.NET Core model binding
/// scenarios.
/// </summary>
internal static class ModelBindingContextTestDoubles
{
    /// <summary>
    /// Creates a binding context using <see cref="DefaultModelBindingContext.CreateBindingContext"/>.
    /// This method simulates the full ASP.NET Core pipeline by wiring up an <see cref="ActionContext"/>
    /// with a fake <see cref="HttpContext"/> and service provider.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceProvider"/> to attach to the <see cref="HttpContext.RequestServices"/>.
    /// Typically built from a test <see cref="ServiceCollection"/>.
    /// </param>
    /// <param name="valueProvider">
    /// The <see cref="IValueProvider"/> that supplies simulated request values.
    /// </param>
    /// <returns>
    /// A <see cref="ModelBindingContext"/> created via <see cref="DefaultModelBindingContext.CreateBindingContext"/>,
    /// configured with metadata for <see cref="object"/> and a model name of "TestModel".
    /// </returns>
    internal static ModelBindingContext CreateBindingContext(
        string modelName,
        IServiceProvider services,
        IValueProvider valueProvider)
    {
        ActionContextMock actionContext = new(services);

        ModelBindingContext context =
            DefaultModelBindingContext
                .CreateBindingContext(
                    actionContext,
                    valueProvider,
                    new EmptyModelMetadataProvider()
                        .GetMetadataForType(typeof(object)),
                    bindingInfo: null,
                    modelName);

        return context;
    }
}

/// <summary>
/// Minimal <see cref="ActionContext"/> mock used to provide a fake <see cref="HttpContext"/>
/// with a configured <see cref="IServiceProvider"/>. This allows unit tests to simulate
/// the ASP.NET Core request pipeline without requiring a real server.
/// </summary>
internal class ActionContextMock : ActionContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ActionContextMock"/> class.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceProvider"/> to assign to <see cref="HttpContext.RequestServices"/>.
    /// </param>
    public ActionContextMock(IServiceProvider services)
    {
        HttpContext =
            new DefaultHttpContext
            {
                RequestServices = services
            };
    }
}

