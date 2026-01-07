using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Edubase.Web.UI.Helpers.ModelBinding.Extensions;

/// <summary>
/// Provides extension methods for working with type metadata.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Attempts to determine the element type of a collection-like type.
    /// Supports arrays, generic collections, and base types implementing IEnumerable.
    /// </summary>
    /// <param name="collectionType">The type to inspect.</param>
    /// <returns>
    /// The element type if identifiable; otherwise, <c>null</c>.
    /// </returns>
    public static Type GetElementType(this Type collectionType)
    {
        // If it's an array, return the element type directly (e.g., int[] → int).
        if (collectionType.IsArray)
        {
            return collectionType.GetElementType();
        }

        // If it's a generic type (e.g., List<T>), return the first generic argument (T).
        if (collectionType.IsGenericType)
        {
            return collectionType.GetGenericArguments().FirstOrDefault();
        }

        // Check the base type in case it's a derived generic collection (e.g., custom class inheriting List<T>).
        Type baseType = collectionType.BaseType;

        // If the base type is generic and implements IEnumerable, return its first generic argument.
        return baseType != null && baseType.IsGenericType &&
               typeof(IEnumerable).IsAssignableFrom(baseType)
            ? baseType.GetGenericArguments().FirstOrDefault()
            : null;
    }

    /// <summary>
    /// Determines whether a given type is considered a "simple" type for model binding or conversion purposes.
    /// Simple types include primitives, enums, strings, decimals, DateTime, and Guid — as well as their nullable equivalents.
    /// </summary>
    /// <param name="type">The type to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the type is a simple type; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsSimpleType(this Type type)
    {
        // If the type is nullable (e.g., int?, DateTime?), get the underlying non-nullable type.
        Type underlying = Nullable.GetUnderlyingType(type) ?? type;

        // Return true if the underlying type is:
        // - a primitive (e.g., int, bool, double)
        // - an enum (e.g., DayOfWeek)
        // - a string
        // - a decimal (not considered primitive by .NET)
        // - a DateTime
        // - a Guid
        return underlying.IsPrimitive
            || underlying.IsEnum
            || underlying == typeof(string)
            || underlying == typeof(decimal)
            || underlying == typeof(DateTime)
            || underlying == typeof(Guid);
    }

    /// <summary>
    /// Determines whether a given type is considered a primitive type for conversion or binding purposes.
    /// This includes .NET primitive types (e.g., int, bool, double) and explicitly includes <see cref="decimal"/>,
    /// which is not classified as primitive by the .NET runtime but is commonly treated as such in application logic.
    /// Nullable types are unwrapped before evaluation.
    /// </summary>
    /// <param name="type">The type to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the type is a primitive or a <see cref="decimal"/>; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsPrimitiveType(this Type type)
    {
        // If the type is nullable (e.g., int?, decimal?), get the underlying non-nullable type.
        Type underlying = Nullable.GetUnderlyingType(type) ?? type;

        // Return true if the underlying type is:
        // - a primitive (e.g., int, bool, double)
        // - or explicitly a decimal (which is not technically primitive but treated as such).
        return underlying.IsPrimitive || underlying == typeof(decimal);
    }

    /// <summary>
    /// Determines whether the specified type represents a collection.
    /// This includes arrays and generic types that implement <see cref="IEnumerable"/>,
    /// but explicitly excludes <see cref="string"/> which also implements <see cref="IEnumerable"/>.
    /// </summary>
    /// <param name="type">The type to evaluate.</param>
    /// <returns>
    /// <c>true</c> if the type is an array or a generic collection type (excluding string); otherwise, <c>false</c>.
    /// </returns>
    public static bool IsCollectionType(this Type type)
    {
        // Return true if the type is an array (e.g., int[], string[]).
        if (type.IsArray)
        {
            return true;
        }

        // Return true if the type is a generic type (e.g., List<T>, IEnumerable<T>)
        // AND it implements IEnumerable
        // AND it is not a string (which technically implements IEnumerable<char> but is not a collection).
        if (type.IsGenericType &&
            typeof(IEnumerable).IsAssignableFrom(type) &&
            type != typeof(string))
        {
            return true;
        }

        // Otherwise, it's not a collection type.
        return false;
    }

    /// <summary>
    /// Resolves a concrete type for a given abstract or interface collection type.
    /// If the type is already concrete, it is returned unchanged.
    /// If the type is a generic collection interface (e.g., IEnumerable&lt;T&gt;, ICollection&lt;T&gt;, IList&lt;T&gt;),
    /// it returns a concrete List&lt;T&gt; type.
    /// </summary>
    /// <param name="type">The type to resolve.</param>
    /// <returns>
    /// A concrete type suitable for instantiation, or the original type if already concrete.
    /// </returns>
    public static Type ResolveConcreteType(this Type type)
    {
        // If the type is already concrete (not an interface or abstract), return it directly.
        if (!type.IsInterface && !type.IsAbstract)
        {
            return type;
        }

        // If the type is not generic, we can't resolve a concrete generic type — return as-is.
        if (!type.IsGenericType)
        {
            return type;
        }

        // Get the generic type definition (e.g., IEnumerable<>, IList<>).
        var genericDef = type.GetGenericTypeDefinition();

        // Get the generic argument (e.g., T in IEnumerable<T>).
        var elementType = type.GetGenericArguments()[0];

        // If the type is a supported collection interface, return a concrete List<T> type.
        if (genericDef == typeof(IEnumerable<>) ||
            genericDef == typeof(ICollection<>) ||
            genericDef == typeof(IList<>))
        {
            return typeof(List<>).MakeGenericType(elementType);
        }

        // If no match, return the original type.
        return type;
    }
}
