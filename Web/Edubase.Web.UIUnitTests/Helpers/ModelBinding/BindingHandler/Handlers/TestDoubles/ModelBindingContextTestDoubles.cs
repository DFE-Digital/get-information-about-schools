using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Edubase.Web.UIUnitTests.Helpers.ModelBinding.BindingHandler.Handlers.TestDoubles
{
    // A static helper class that provides test doubles for creating DefaultModelBindingContext instances.
    // This is used in unit tests to avoid repeating boilerplate setup code.
    internal static class ModelBindingContextTestDoubles
    {
        /// <summary>
        /// Creates a stubbed DefaultModelBindingContext for unit testing.
        /// </summary>
        /// <param name="valueProvider">
        /// The IValueProvider to use for supplying values during model binding.
        /// Typically mocked in tests to simulate incoming request data.
        /// </param>
        /// <param name="type">
        /// The model type for which metadata should be generated.
        /// </param>
        /// <returns>
        /// A DefaultModelBindingContext configured with the provided value provider and model metadata.
        /// </returns>
        internal static DefaultModelBindingContext Stub(
            IValueProvider valueProvider, Type type) =>
                new()
                {
                    // The source of values (e.g., query string, form data) used during binding.
                    ValueProvider = valueProvider,

                    // Metadata about the model type (properties, attributes, etc.).
                    // EmptyModelMetadataProvider is used here to generate metadata for the given type.
                    ModelMetadata =
                        new EmptyModelMetadataProvider()
                            .GetMetadataForType(type)
                };
    }
}
