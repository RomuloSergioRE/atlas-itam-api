using Atlas.Itam.Application.Common.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Atlas.Itam.Infrastructure.Services;

public sealed class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public SmtpEmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(
            _configuration["EmailSettings:FromName"] ?? "Atlas ITAM",
            _configuration["EmailSettings:FromEmail"]));
        message.To.Add(new MailboxAddress("", to));
        message.Subject = subject;

        message.Body = new TextPart("html") { Text = body };

        using var client = new SmtpClient();
        if (!string.IsNullOrEmpty(_configuration["EmailSettings:SmtpServer"]))
        {
            await client.ConnectAsync(
                _configuration["EmailSettings:SmtpServer"]!,
                int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587"),
                MailKit.Security.SecureSocketOptions.StartTls,
                cancellationToken);

            var user = _configuration["EmailSettings:SmtpUser"];
            var pass = _configuration["EmailSettings:SmtpPassword"];
            if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pass))
                await client.AuthenticateAsync(user, pass, cancellationToken);

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }
    }
}
