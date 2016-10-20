using Edubase.Data.Entity;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System.Configuration;
using System.Threading.Tasks;

namespace Edubase.Services
{
    public class BusMessagingService
    {
        public async Task SendEstablishmentUpdateMessageAsync(Establishment establishment)
        {
            var title = establishment.HeadTitleId.HasValue ? new LookupService().GetName("HeadTitleId", establishment.HeadTitleId.Value) : null as string;
            var client = QueueClient.CreateFromConnectionString(ConfigurationManager.ConnectionStrings["ServiceBusConnectionString"].ConnectionString, "updates");
            var payload = JsonConvert.SerializeObject(new
            {
                HeadTitle = title,
                HeadFirstName = establishment.HeadFirstName,
                HeadLastName = establishment.HeadLastName,
                URN = establishment.Urn,
                Name = establishment.Name
            });
            var message = new BrokeredMessage(payload);
            await client.SendAsync(message);

            MessageLoggingService.Instance.Push(new LogMessage
            {
                Level = LogMessage.eLevel.Information,
                Text = "Estab upd msg sent: " + payload
            });
        }
    }
}
