using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using TangerineAuction.Core.Services;
using TangerineAuction.Core.UseCases.Auctions;
using TangerineAuction.Core.UseCases.Auctions.Dtos;
using TangerineAuction.Core.UseCases.Bets;
using TangerineAuction.Core.UseCases.Bets.Dtos;
using TangerineAuction.Core.UseCases.Tangerines;
using TangerineAuction.Core.UseCases.Tangerines.Dtos;
using TangerineAuction.Shared;

namespace TangerineAuction.Infrastructure.Integration;

internal class OnTangerineCreatedConsumer(
    ISender sender,
    ISystemUserService systemUserService,
    ILogger<OnTangerineCreatedConsumer> logger) 
    : IConsumer<OnTangerineCreatedRequest>
{
    public async Task Consume(ConsumeContext<OnTangerineCreatedRequest> context)
    {
        var tangerine = context.Message.TangerineInfo;
        
        var addTangerineResult = await sender.Send(new AddTangerine.Command(new TangerineRequest
        {
            Name = tangerine.Name,
            Quality = tangerine.Quality,
            StartPrice = tangerine.StartPrice,
            FilePath = tangerine.FilePath
        }), context.CancellationToken);
        if (!addTangerineResult.IsSuccess)
        {
            logger.LogError($"Не удалось создать мандарин ({tangerine}). Ошибки: \n {string.Join(Environment.NewLine, addTangerineResult.Errors)}");
            return;
        }
        
        var addAuctionResult = await sender.Send(new AddAuction.Command(new AuctionRequest
        {
            Name = $"Аукцион №{tangerine.CreatedOn:yyyyMMddHHmmss}",
            TangerineId = addTangerineResult.Value.Id
        }), context.CancellationToken);
        if (!addTangerineResult.IsSuccess)
        {
            logger.LogError($"Не удалось создать аукцион. Ошибки: \n {string.Join(Environment.NewLine, addAuctionResult.Errors)}");
            return;
        }

        var request = new BetRequest(addAuctionResult.Value.Id, tangerine.StartPrice);
        request.SetCreatedBy(systemUserService.UserId);
        var addBetResult = await sender.Send(new AddBet.Command(request), context.CancellationToken);
        if (!addTangerineResult.IsSuccess)
        {
            logger.LogError($"Не удалось создать ставку. Ошибки: \n {string.Join(Environment.NewLine, addBetResult.Errors)}");
            return;
        }
        
        logger.LogInformation($"Создан мандарин, аукцион ({addAuctionResult.Value.Name}) и ставка");
    }
}