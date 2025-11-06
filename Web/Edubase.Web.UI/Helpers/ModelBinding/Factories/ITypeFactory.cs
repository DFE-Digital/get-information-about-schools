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
    /// Creates an instance of the specified type.
    /// </summary>
    /// <param name="type">
    /// The <see cref="Type"/> to instantiate. This may be a concrete class, a generic type,
    /// or a resolved implementation of an abstract or interface type.
    /// </param>
    /// <returns>
    /// A new instance of the specified type, or throws an exception if the type cannot be instantiated.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the provided <paramref name="type"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the type is abstract, an interface, or lacks a public parameterless constructor.
    /// </exception>
    object CreateInstance(Type type);
}

