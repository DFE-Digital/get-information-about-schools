using System.Net.Mail;
using System.Threading.Tasks;

namespace Edubase.Services.IntegrationEndPoints.Smtp
{
    public class MockSmtpEndPoint : ISmtpEndPoint
    {
        public Task SendAsync(MailMessage msg) => Task.CompletedTask;

        public Task SendAsync(string from, string to, string subject, string body) => Task.CompletedTask;
    }
}
