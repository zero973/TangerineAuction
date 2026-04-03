using Ardalis.Result;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using TangerineAuction.Core.Enums;
using TangerineAuction.Core.Services;
using TangerineAuction.Core.UseCases.Auctions;
using TangerineAuction.Core.UseCases.Auctions.Dtos;
using TangerineAuction.Core.UseCases.Bets;
using TangerineAuction.Core.UseCases.Bets.Dtos;
using TangerineAuction.Core.UseCases.SignalR;
using TangerineAuction.Core.UseCases.Tangerines;
using TangerineAuction.Core.UseCases.Tangerines.Dtos;
using TangerineAuction.Shared.Models;
using TangerineGenerator.Shared;

namespace TangerineAuction.Infrastructure.Integration;

internal class GenerateTangerineRequestHandler(
    IRequestClient<GenerateTangerineRequest> requestClient,
    ISender sender,
    ISystemUserService systemUserService,
    ILogger<GenerateTangerineRequestHandler> logger)
    : IRequestHandler<GenerateTangerine.Command, Result>
{
    public async Task<Result> Handle(GenerateTangerine.Command request, CancellationToken ct)
    {
        try
        {
            var tangerine = (await requestClient.GetResponse<TangerineInfo>(new GenerateTangerineRequest(), ct)).Message;
            
            var addTangerineResult = await sender.Send(new AddTangerine.Command(new TangerineRequest(tangerine)), ct);
            if (!addTangerineResult.IsSuccess)
            {
                logger.LogError("Не удалось создать мандарин ({TangerineInfo}). Ошибки: \n {Join}", 
                    tangerine, string.Join(Environment.NewLine, addTangerineResult.Errors));
                return Result.Error(new ErrorList(addTangerineResult.Errors));
            }
        
            var addAuctionResult = await sender.Send(new AddAuction.Command(new AuctionRequest
            {
                Name = $"Аукцион №{tangerine.CreatedOn:yyyyMMddHHmmss}",
                TangerineId = addTangerineResult.Value.Id
            }), ct);
            if (!addAuctionResult.IsSuccess)
            {
                logger.LogError("Не удалось создать аукцион. Ошибки: \n {Join}", 
                    string.Join(Environment.NewLine, addAuctionResult.Errors));
                return Result.Error(new ErrorList(addAuctionResult.Errors));
            }

            var betRequest = new BetRequest(addAuctionResult.Value.Id, tangerine.StartPrice)
            {
                CreatedBy = systemUserService.UserId
            };
            var addBetResult = await sender.Send(new AddBet.Command(betRequest), ct);
            if (!addBetResult.IsSuccess)
            {
                logger.LogError("Не удалось создать ставку. Ошибки: \n {Join}", 
                    string.Join(Environment.NewLine, addBetResult.Errors));
                return Result.Error(new ErrorList(addBetResult.Errors));
            }
        
            await sender.Send(new SendMessage.Command(HubMessageType.AuctionAdded, addAuctionResult.Value.Id), ct);
            
            logger.LogInformation("Создан мандарин, аукцион ({ValueName}) и ставка", addAuctionResult.Value.Name);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Произошла ошибка при генерации мандаринки");
            return Result.Error(e.Message);
        }
        
        return Result.Success();
    }
}