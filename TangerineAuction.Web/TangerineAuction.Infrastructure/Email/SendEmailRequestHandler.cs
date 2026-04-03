using Ardalis.Result;
using MediatR;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;
using TangerineAuction.Core.UseCases.Email;

namespace TangerineAuction.Infrastructure.Email;

internal class SendEmailRequestHandler(
    IOptions<SmtpOptions> smtpOptions,
    IHttpClientFactory httpClientFactory,
    ILogger<SendEmailRequestHandler> logger)
    : IRequestHandler<SendEmail.Command, Result>
{
    public async Task<Result> Handle(SendEmail.Command request, CancellationToken ct)
    {
        var options = smtpOptions.Value;

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(options.FromName, options.FromEmail));
        message.To.Add(MailboxAddress.Parse(request.Receiver));
        message.Subject = request.Topic;

        var multipart = new Multipart("related");

        var htmlBody = new TextPart("html")
        {
            Text = $"""
                    <div style="white-space: pre-wrap; font-family: Arial, sans-serif;">
                    {request.Body}
                    </div>
                    <img src="cid:image1" />
                    """
        };

        multipart.Add(htmlBody);

        try
        {
            if (!string.IsNullOrWhiteSpace(request.ImageUrl))
            {
                var httpClient = httpClientFactory.CreateClient();
                var imageBytes = await httpClient.GetByteArrayAsync(request.ImageUrl, ct);
                var imageStream = new MemoryStream(imageBytes);

                var attachment = new MimePart("image", "png")
                {
                    Content = new MimeContent(imageStream),
                    ContentDisposition = new ContentDisposition(ContentDisposition.Inline),
                    ContentTransferEncoding = ContentEncoding.Base64,
                    ContentId = "image1",
                    FileName = Path.GetFileName(new Uri(request.ImageUrl).LocalPath)
                };

                multipart.Add(attachment);
            }

            message.Body = multipart;

            var secureSocketOption = options.UseSsl
                ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTlsWhenAvailable;

            using var client = new SmtpClient();
            await client.ConnectAsync(options.Host, options.Port, secureSocketOption, ct);
            await client.AuthenticateAsync(options.UserName, options.Password, ct);
            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);

            return Result.Success();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при отправке сообщения");
            return Result.Error("Ошибка при отправке сообщения");
        }
    }
}