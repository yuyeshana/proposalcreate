using System.Net;
using System.Net.Mail;

namespace Proposal.Services
{
    public class EmailSender
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _fromEmail;
        private readonly string _fromPassword;

        public EmailSender(string smtpHost, int smtpPort, string fromEmail, string fromPassword)
        {
            _smtpHost = smtpHost;
            _smtpPort = smtpPort;
            _fromEmail = fromEmail;
            _fromPassword = fromPassword;
        }

        public void Send(string toEmail, string subject, string body)
        {
            using var client = new SmtpClient(_smtpHost)
            {
                Port = _smtpPort,
                Credentials = new NetworkCredential(_fromEmail, _fromPassword),
                EnableSsl = true
            };

            var mail = new MailMessage(_fromEmail, toEmail, subject, body);
            client.Send(mail);
        }
    }
}
