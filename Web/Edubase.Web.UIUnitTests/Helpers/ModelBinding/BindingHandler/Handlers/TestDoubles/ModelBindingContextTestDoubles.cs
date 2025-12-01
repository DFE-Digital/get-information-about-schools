using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Edubase.Web.UIUnitTests.Helpers.ModelBinding.BindingHandler.Handlers.TestDoubles;

/// <summary>
/// Provides reusable test doubles for creating <see cref="DefaultModelBindingContext"/> instances.
/// This helper is intended for unit tests to avoid repeating boilerplate setup code
/// when constructing a model binding context.
/// </summary>
internal static class ModelBindingContextTestDoubles
{
    /// <summary>
    /// Creates a stubbed <see cref="DefaultModelBindingContext"/> for unit testing.
    /// </summary>
    internal static DefaultModelBindingContext Stub(
        IValueProvider valueProvider, Type type) =>
        new()
        {
            ModelName = string.Empty,
            ValueProvider = valueProvider,
            ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(type),
            Result = ModelBindingResult.Success(null)
        };

    /// <summary>
    /// Convenience stub: builds a <see cref="DefaultModelBindingContext"/> from a dictionary of values.
    /// </summary>
    /// <typeparam name="T">The model type for which metadata should be generated.</typeparam>
    /// <param name="values">Dictionary of key → string[] values representing simulated request data.</param>
    /// <returns>
    /// A <see cref="DefaultModelBindingContext"/> configured with a <see cref="ValueProviderTestDoubles.TestValueProvider"/>
    /// and metadata for the specified model type.
    /// </returns>
    internal static DefaultModelBindingContext StubForDictionary<TObject>(
        Dictionary<string, string[]> values) =>
        new()
        {
            ModelName = string.Empty,
            ValueProvider = new ValueProviderTestDoubles.TestValueProvider(values),
            ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(typeof(TObject)),
            Result = ModelBindingResult.Success(Activator.CreateInstance<TObject>())
        };
}
