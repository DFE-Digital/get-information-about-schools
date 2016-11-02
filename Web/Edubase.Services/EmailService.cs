using Edubase.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services
{
    public class EmailService
    {
        public async Task SendAsync(string from, string to, string subject, string body)
        {
            var client = new SmtpClient();
            await Retry.RetryableActionAsync(async () => await client.SendMailAsync(from, to, subject, body));
        }

        public async Task SendAsync(MailMessage msg)
        {
            var client = new SmtpClient();
            await Retry.RetryableActionAsync(async () => await client.SendMailAsync(msg));
        }
    }
}
