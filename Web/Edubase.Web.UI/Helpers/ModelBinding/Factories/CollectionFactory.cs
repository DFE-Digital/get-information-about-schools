using System;
using System.Collections;
using System.Collections.Generic;

namespace Edubase.Web.UI.Helpers.ModelBinding.Factories;

/// <summary>
/// Provides functionality to create collection instances from a specified type and populate them with items.
/// Supports arrays, generic interfaces (e.g., IEnumerable&lt;T&gt;), and concrete IList implementations.
/// </summary>
public class CollectionFactory(
    ITypeFactory typeFactory) : ICollectionFactory
{
    /// <summary>
    /// Creates a collection instance of the specified type and populates it with the provided items.
    /// </summary>
    /// <param name="listType">The target collection type to instantiate (e.g., List&lt;T&gt;, int[]).</param>
    /// <param name="elementType">The type of elements contained in the collection.</param>
    /// <param name="items">The items to populate the collection with.</param>
    /// <returns>A populated collection instance matching the specified type.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the specified type cannot be instantiated or is unsupported.
    /// </exception>
    public object CreateListInstance(Type listType, Type elementType, object[] items)
    {
        // Handle array types (e.g., int[], string[])
        if (listType.IsArray)
        {
            Array array = Array.CreateInstance(elementType, items.Length);
            items.CopyTo(array, 0);
            return array;
        }

        // Handle abstract or interface types by falling back to List<T>
        if (listType.IsInterface || listType.IsAbstract)
        {
            return CreateAndPopulateList(
                concreteType: typeof(List<>).MakeGenericType(elementType), items);
        }

        // Handle concrete types that implement IList
        if (typeof(IList).IsAssignableFrom(listType))
        {
            return CreateAndPopulateList(
                concreteType: listType, items);
        }

        // If none of the above, the type is unsupported
        throw new InvalidOperationException(
            $"Unable to create a list instance for type {listType.FullName}");
    }

    /// <summary>
    /// Instantiates a list of the specified type and populates it with the given items.
    /// </summary>
    /// <param name="concreteType">The concrete list type to instantiate.</param>
    /// <param name="items">The items to add to the list.</param>
    /// <returns>A populated IList instance.</returns>
    private object CreateAndPopulateList(Type concreteType, object[] items)
    {
        object instance =
            typeFactory.CreateInstance(concreteType);

        if (instance is not IList list){
            throw new InvalidOperationException(
                $"Type {concreteType.FullName} does not implement IList.");
        }

        foreach (object item in items){
            list.Add(item);
        }

        return list;
    }
}
