using Edubase.Common;
using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Edubase.Services.Domain;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services
{
    public class LogMessageReadService
    {
        public async Task<LogMessagesDto> GetAllAsync(int take, string skipToken = null, DateTime? date = null)
        {
            var result = await new LogMessageRepository().GetAllAsync(take, UriHelper.TryDeserializeUrlToken<TableContinuationToken>(skipToken), date);
            return new LogMessagesDto(result.Item1, UriHelper.SerializeToUrlToken(result.Item2));
        }

        public async Task<LogMessage> GetAsync(string id) => await new LogMessageRepository().GetAsync(id);

        public async Task<LogMessage> GetAsync(string partitionKey, string rowKey) => await new LogMessageRepository().GetAsync(partitionKey, rowKey);
    }
}
