using Ardalis.Result;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using TangerineAuction.Core.UseCases.SignalR;

namespace TangerineAuction.Server.SignalR;

internal class SendMessageRequestHandler(
    IHubContext<AuctionHub> hubContext, 
    IConfiguration configuration, 
    ILogger<SendMessageRequestHandler> logger) 
    : IRequestHandler<SendMessage.Command, Result>
{
    public async Task<Result> Handle(SendMessage.Command request, CancellationToken ct)
    {
        try
        {
            await hubContext.Clients.All.SendAsync(
                configuration["SignalR:ReceiveMethodName"]!,
                $"{request.Type}:{request.EntityId}",
                ct);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при отправке сообщения в Hub. {EMessage}", e.Message);
            return Result.Error($"Ошибка при отправке сообщения в Hub. {e.Message}");
        }
        
        return Result.Success();
    }
}