using System;
using System.Collections;
using System.Collections.Generic;
using Edubase.Web.UI.Helpers.ModelBinding.Factories;

namespace Edubase.Web.UIUnitTests.Helpers.ModelBinding.TypeConverters.TestDoubles;

/// <summary>
/// Provides reusable test doubles for <see cref="ICollectionFactory"/>.
/// These helpers simplify unit test setup by offering fake implementations
/// that return predictable collection instances.
/// </summary>
internal static class CollectionFactoryTestDoubles
{
    /// <summary>
    /// A fake implementation of <see cref="ICollectionFactory"/> for unit testing.
    /// Supports arrays and generic <see cref="List{T}"/>.
    /// </summary>
    internal sealed class FakeCollectionFactoryDouble : ICollectionFactory
    {
        /// <summary>
        /// Creates a collection instance of the specified type and populates it with items.
        /// Supports arrays and generic lists. Throws <see cref="InvalidOperationException"/> for unsupported types.
        /// </summary>
        /// <param name="listType">The collection type to create (array or List&lt;T&gt;).</param>
        /// <param name="elementType">The element type of the collection.</param>
        /// <param name="items">The items to populate the collection with.</param>
        /// <returns>A populated collection instance.</returns>
        public object CreateListInstance(Type listType, Type elementType, object[] items)
        {
            if (listType.IsArray)
            {
                Array array =
                    Array.CreateInstance(
                        elementType, items.Length);

                items.CopyTo(array, 0);
                return array;
            }

            if (listType.IsGenericType && listType
                .GetGenericTypeDefinition() == typeof(List<>))
            {
                IList list =
                    (IList)Activator
                        .CreateInstance(
                            typeof(List<>).MakeGenericType(elementType));

                foreach (object item in items)
                {
                    list.Add(item);
                }

                return list;
            }

            throw new InvalidOperationException(
                "Unsupported collection type");
        }
    }
}
