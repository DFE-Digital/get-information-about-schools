using System;
using Edubase.Web.UI.Helpers.ModelBinding.Extensions;

namespace Edubase.Web.UI.Helpers.ModelBinding.Factories;

/// <summary>
/// Provides functionality for dynamically creating instances of types at runtime.
/// Supports arrays and abstract/interface types by resolving them to concrete implementations.
/// </summary>
public class TypeFactory : ITypeFactory
{
    /// <summary>
    /// Creates an instance of the specified type.
    /// If the type is an array, it creates an empty array of the appropriate element type.
    /// If the type is abstract or an interface, it attempts to resolve a concrete type using <see cref="ResolveConcreteType"/>.
    /// </summary>
    /// <param name="type">
    /// The type to instantiate. This can be a concrete class, array type, or an abstract/interface type
    /// that can be resolved to a concrete implementation.
    /// </param>
    /// <returns>
    /// A new instance of the specified or resolved concrete type.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the provided <paramref name="type"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the resolved type cannot be instantiated (e.g., no parameterless constructor).
    /// </exception>
    public object CreateInstance(Type type)
    {
        // Handle array types by creating an empty array of the appropriate element type
        if (type.IsArray)
        {
            // Get the type of elements stored in the array (e.g., int[] → int)
            Type elementType = type.GetElementType();

            // Create an empty array of the element type
            return Array.CreateInstance(elementType, 0);
        }

        // Resolve abstract/interface types to a concrete implementation if necessary
        Type concreteType = type.ResolveConcreteType();

        // Create an instance of the resolved concrete type using its default constructor
        return Activator.CreateInstance(concreteType);
    }
}
