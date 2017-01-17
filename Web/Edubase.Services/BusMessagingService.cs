//using Edubase.Data.Entity;
//using Microsoft.ServiceBus.Messaging;
//using Newtonsoft.Json;
//using System;
//using System.Configuration;
//using System.Threading.Tasks;

//namespace Edubase.Services
//{
//    public class BusMessagingService
//    {
//        public async Task SendEstablishmentUpdateMessageAsync(Establishment establishment)
//        {
//            throw new NotImplementedException();

//            //var title = await (establishment.HeadTitleId.HasValue 
//            //    ? new CachedLookupService().GetNameAsync("HeadTitleId", establishment.HeadTitleId.Value) 
//            //    : Task.FromResult(null as string));

//            //var client = QueueClient.CreateFromConnectionString(ConfigurationManager.ConnectionStrings["ServiceBusConnectionString"].ConnectionString, "updates");

//            //var payload = JsonConvert.SerializeObject(new
//            //{
//            //    HeadTitle = title,
//            //    HeadFirstName = establishment.HeadFirstName,
//            //    HeadLastName = establishment.HeadLastName,
//            //    URN = establishment.Urn,
//            //    Name = establishment.Name
//            //});
//            //var message = new BrokeredMessage(payload);
//            //await client.SendAsync(message);

//            //MessageLoggingService.Instance.Push(new LogMessage
//            //{
//            //    Level = LogMessage.eLevel.Information,
//            //    Text = "Estab upd msg sent: " + payload
//            //});
//        }

//        /// <summary>
//        /// Broadcasts a message to all listeners.
//        /// </summary>
//        /// <param name="topic"></param>
//        /// <param name="message"></param>
//        /// <returns></returns>
//        public async Task Broadcast(string topic, string message)
//        {
//            throw new NotImplementedException();
//        }

//    }
//}
