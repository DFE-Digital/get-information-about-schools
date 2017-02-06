using System.Threading.Tasks;
using Edubase.Data.Entity;

namespace Edubase.Services.IntegrationEndPoints.ServiceBus
{
    public interface IServiceBusEndPoint
    {
        Task SendEstablishmentUpdateMessageAsync(Establishment establishment);
    }
}