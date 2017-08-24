using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;

namespace Edubase.Data.Repositories.TableStorage
{
    public class Page<T>
    {
        public Page(IEnumerable<T> items, TableContinuationToken tableContinuationToken)
        {
            TableContinuationToken = tableContinuationToken;
            Items = items;
        }

        public Page()
        {

        }

        public TableContinuationToken TableContinuationToken { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}
