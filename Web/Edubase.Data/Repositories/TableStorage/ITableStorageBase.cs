using Microsoft.WindowsAzure.Storage.Table;

namespace Edubase.Data.Repositories.TableStorage
{
    public interface ITableStorageBase<T>
    {
        CloudTable Table { get; }
    }
}
