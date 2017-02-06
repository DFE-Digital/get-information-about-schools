using Edubase.Data.Entity;
using Edubase.Services.Lookup;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace Edubase.Services.IntegrationEndPoints.ServiceBus
{
    public class ServiceBusEndPoint : IServiceBusEndPoint
    {
        private IMessageLoggingService _messageLoggingService;
        private ICachedLookupService _lookup;

        public ServiceBusEndPoint(IMessageLoggingService messageLoggingService, ICachedLookupService lookup)
        {
            _messageLoggingService = messageLoggingService;
            _lookup = lookup;
        }

        public async Task SendEstablishmentUpdateMessageAsync(Establishment establishment)
        {   
            //var title = await (establishment.HeadTitleId.HasValue
            //    ? _lookup.GetNameAsync("HeadTitleId", establishment.HeadTitleId.Value)
            //    : Task.FromResult(null as string));

            //var client = QueueClient.CreateFromConnectionString(ConfigurationManager.ConnectionStrings["ServiceBusConnectionString"].ConnectionString, "updates");

            //var payload = JsonConvert.SerializeObject(new
            //{
            //    HeadTitle = title,
            //    HeadFirstName = establishment.HeadFirstName,
            //    HeadLastName = establishment.HeadLastName,
            //    URN = establishment.Urn,
            //    Name = establishment.Name
            //});
            //var message = new BrokeredMessage(payload);
            //await client.SendAsync(message);

            //_messageLoggingService.Push(new LogMessage
            //{
            //    Level = LogMessage.eLevel.Information,
            //    Text = "Estab upd msg sent: " + payload
            //});
        }

    }
}
