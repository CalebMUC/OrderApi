using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Minimart_Api.Services.EmailServices
{
    public class BrevoEmailService
    {
        public async Task SendAsync(string to, string subject, string body) {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("MinimartKe", "calebmuchiri04@gmail.com"));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;

            message.Body = new TextPart("plain") { Text = body };


            using var Client = new SmtpClient();
            //Connect to brevo smtp server, on port 587 
            await Client.ConnectAsync("smtp-relay.brevo.com", 587, SecureSocketOptions.StartTls);
            await Client.AuthenticateAsync("8b33f8001@smtp-brevo.com", "O3ACj5rmPbgTH1VR");
            await Client.SendAsync(message);
            await Client.DisconnectAsync(true);
        }
    }
}
