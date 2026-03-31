using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;
using TangerineAuction.Core.UseCases.Email;

namespace TangerineAuction.Infrastructure.Email;

internal class SendEmailRequestHandler(IOptions<SmtpOptions> smtpOptions, ILogger<SendEmailRequestHandler> logger) 
    : IRequestHandler<SendEmail.Command, Result>
{
    public async Task<Result> Handle(SendEmail.Command request, CancellationToken ct)
    {
        var options = smtpOptions.Value;

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(options.FromName, options.FromEmail));
        message.To.Add(MailboxAddress.Parse(request.Receiver));
        message.Subject = request.Topic;

        var body = new TextPart("plain")
        {
            Text = request.Body
        };

        var multipart = new Multipart("mixed");
        multipart.Add(body);


        if (request.TangerineFilePath != null)
        {
            var attachment = new MimePart("image", "png")
            {
                Content = new MimeContent(File.OpenRead(request.TangerineFilePath)),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = Path.GetFileName(request.TangerineFilePath)
            };

            multipart.Add(attachment);
        }

        message.Body = multipart;

        using var client = new SmtpClient();

        var secureSocketOption = options.UseSsl
            ? SecureSocketOptions.SslOnConnect
            : SecureSocketOptions.StartTlsWhenAvailable;

        try
        {
            await client.ConnectAsync(options.Host, options.Port, secureSocketOption, ct);

            if (!string.IsNullOrWhiteSpace(options.UserName))
            {
                await client.AuthenticateAsync(options.UserName, options.Password, ct);
            }

            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при отправке сообщения");
            return Result.Error("Ошибка при отправке сообщения");
        }

        return Result.Success();
    }
}