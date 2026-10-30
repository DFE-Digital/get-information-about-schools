using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Edubase.Web.UIUnitTests.Helpers.ModelBinding.TestDoubles;

/// <summary>
/// Test double implementation of <see cref="ModelBinderProviderContext"/>.
/// Provides minimal metadata and binder creation behavior for unit testing.
/// </summary>
internal class ModelBinderProviderContextTestDoubles(
    Type modelType) : ModelBinderProviderContext
{
    /// <summary>
    /// Gets the model metadata for the test context.
    /// </summary>
    public override ModelMetadata Metadata { get; } =
        new EmptyModelMetadataProvider().GetMetadataForType(modelType);

    /// <summary>
    /// Provides binding info that tests can configure
    /// </summary>
    public override BindingInfo BindingInfo { get; } = new();

    /// <summary>
    /// Returns the metadata provider used to generate <see cref="Metadata"/>.
    /// </summary>
    public override IModelMetadataProvider MetadataProvider { get; } = new EmptyModelMetadataProvider();

    /// <summary>
    /// Returns a dummy binder that simply throws if invoked.
    /// Override in tests if you need a real binder.
    /// </summary>
    public override IModelBinder CreateBinder(ModelMetadata metadata)
    {
        throw new NotSupportedException(
            "CreateBinder is not supported in this test double.");
    }
}

