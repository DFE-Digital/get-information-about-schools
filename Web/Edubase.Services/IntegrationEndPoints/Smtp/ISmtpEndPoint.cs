using System.Net.Mail;
using System.Threading.Tasks;

namespace Edubase.Services.IntegrationEndPoints.Smtp
{
    public interface ISmtpEndPoint
    {
        Task SendAsync(MailMessage msg);
        Task SendAsync(string from, string to, string subject, string body);
    }
}