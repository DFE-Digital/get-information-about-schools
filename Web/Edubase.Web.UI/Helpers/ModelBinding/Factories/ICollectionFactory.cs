using System;

namespace Edubase.Web.UI.Helpers.ModelBinding.Factories
{
    public interface ICollectionFactory
    {
        object CreateListInstance(Type listType, Type elementType, object[] items);
    }
}
