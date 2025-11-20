using System;

namespace Edubase.Web.UI.Helpers.ModelBinding.Factories;

/// <summary>
/// Defines a contract for creating instances of types at runtime.
/// This interface is typically used in scenarios such as model binding, dependency resolution,
/// or dynamic object construction where the type is known but must be instantiated programmatically.
/// </summary>
public interface ITypeFactory
{
    /// <summary>
    /// Creates an instance of the specified type using its parameterless constructor.
    /// </summary>
    object CreateInstance(Type type);

    /// <summary>
    /// Creates an instance of the specified type using a
    /// constructor that matches the given argument types.
    /// </summary>
    object CreateInstance(Type type, params object[] args);

    /// <summary>
    /// Generic convenience overload for parameterless constructors.
    /// </summary>
    TObject CreateInstance<TObject>();
}

