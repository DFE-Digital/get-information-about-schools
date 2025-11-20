using System;

namespace Edubase.Web.UI.Helpers.ModelBinding.Factories;

/// <summary>
/// Defines a factory for creating collection instances during model binding.
/// </summary>
public interface ICollectionFactory
{
    /// <summary>
    /// Creates and returns a new list instance of the specified type and element type,
    /// populated with the provided items.
    /// </summary>
    /// <param name="listType">
    /// The concrete list type to create (e.g., typeof(List&lt;T&gt;)).
    /// </param>
    /// <param name="elementType">
    /// The type of elements contained in the list.
    /// </param>
    /// <param name="items">
    /// An array of items to populate the list with.
    /// </param>
    /// <returns>
    /// A new list instance of the specified type containing the provided items.
    /// </returns>
    object CreateListInstance(Type listType, Type elementType, object[] items);
}
