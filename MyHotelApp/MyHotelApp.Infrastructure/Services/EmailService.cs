using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MyHotelApp.Core.Entities;
using MyHotelApp.Core.Interfaces;

namespace MyHotelApp.Infrastructure.Services;

public class EmailService(IOptions<EmailSettings> emailSettings) : IEmailService
{
    private readonly EmailSettings _emailSettings = emailSettings.Value;

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;

        var builder = new BodyBuilder { HtmlBody = body };
        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        try
        {
            await smtp.ConnectAsync(
                _emailSettings.SmtpServer, 
                _emailSettings.SmtpPort, 
                _emailSettings.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls);

            if (!string.IsNullOrEmpty(_emailSettings.SmtpUser))
            {
                await smtp.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpPass);
            }
            else
            {
                // Optional: You might want to throw if your provider requires auth
                // throw new InvalidOperationException("SMTP User is not configured in appsettings.json");
            }

            await smtp.SendAsync(email);
        }
        catch (Exception ex)
        {
            // Throw a more descriptive exception to be caught by the controller
            throw new Exception($"Email service failed: {ex.Message}. Check your SMTP settings in appsettings.json.", ex);
        }
        finally
        {
            await smtp.DisconnectAsync(true);
        }
    }
}
