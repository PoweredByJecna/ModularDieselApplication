using MimeKit;
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Entities;

namespace ModularDieselApplication.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        // ----------------------------------------
        // Send an email for dieslovani with a specific result.
        // ----------------------------------------
        public async Task SendDieslovaniEmailAsync(Dieslovani dieslovani, string emailResult)
        {
            var subject = "";
            var body = "";

            if (emailResult == "DA-ok")
            {
                subject = $"Objednávka DA č. {dieslovani.ID} " +
                          $"na lokalitu: {dieslovani.Odstavka?.Lokality?.Nazev}";

                body = $@"
                <h1>Dobrý den</h1>
                <p>
                    Toto je objednávka DA na lokalitu: 
                    <strong>{dieslovani.Odstavka?.Lokality?.Nazev}</strong>
                </p>
                ";
            }
            else
            {
                subject = $"Zrušení DA č. {dieslovani.ID} " +
                          $"na lokalitu: {dieslovani.Odstavka?.Lokality?.Nazev}";

                body = $@"
                <h1>Dobrý den</h1>
                <p>
                    Toto je zrušení DA na lokalitu: 
                    <strong>{dieslovani.Odstavka?.Lokality?.Nazev}</strong>
                </p>
                ";
            }

            // Call the general method to send the email.
            await SendEmailAsync(subject, body);
        }

        // ----------------------------------------
        // Send a generic email with a subject and body.
        // ----------------------------------------
        public async Task SendEmailAsync(string subject, string body)
        {
            var emailSettings = _config.GetSection("EmailSettings");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                emailSettings["SenderName"],
                emailSettings["SenderEmail"]));

            // Example: send to the configured recipient
            message.To.Add(new MailboxAddress(
                "", 
                emailSettings["SenderEmail"]));

            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body
            };
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(
                    emailSettings["SmtpServer"],
                    int.TryParse(emailSettings["SmtpPort"], out var smtpPort) ? smtpPort : throw new InvalidOperationException("Invalid SMTP port"),
                    SecureSocketOptions.StartTls
                );

                await client.AuthenticateAsync(
                    emailSettings["Username"], 
                    emailSettings["Password"]);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}