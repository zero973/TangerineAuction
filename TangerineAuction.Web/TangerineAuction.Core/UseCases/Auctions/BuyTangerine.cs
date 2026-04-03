using Ardalis.Result;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using TangerineAuction.Core.Models;
using TangerineAuction.Core.Repository;
using TangerineAuction.Core.Services;
using TangerineAuction.Core.UseCases.Auctions.Dtos;
using TangerineAuction.Core.UseCases.Auctions.Specifications;
using TangerineAuction.Core.UseCases.Auctions.Validators;
using TangerineAuction.Core.UseCases.Bets;
using TangerineAuction.Core.UseCases.Bets.Dtos;

namespace TangerineAuction.Core.UseCases.Auctions;

/// <summary>
/// Выкупить лот
/// </summary>
public class BuyTangerine
{
    
    public record Command(BuyTangerineRequest Request) : IRequest<Result>;

    internal class RequestHandler(
        IReadRepository<Auction> auctionRepository,
        ISender sender, 
        ICurrentUserService currentUserService,
        ILogger<RequestHandler> logger) 
        : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken ct)
        {
            var buyPrice = await auctionRepository.FirstOrDefaultAsync(
                new TangerineBuyPriceSpecification(request.Request.Id), ct);
            var betRequest = new BetRequest(request.Request.Id, buyPrice)
            {
                CreatedBy = currentUserService.UserId
            };
            var addBetResult = await sender.Send(new AddBet.Command(betRequest), ct);
            if (!addBetResult.IsSuccess)
            {
                logger.LogError("Не удалось создать ставку. Ошибки: \n {Join}", 
                    string.Join(Environment.NewLine, addBetResult.Errors));
                return Result.Error(new ErrorList(addBetResult.Errors));
            }
            
            var finishAuctionResult = await sender.Send(new FinishAuction.Command(request.Request.Id), ct);
            if (!finishAuctionResult.IsSuccess)
            {
                logger.LogError("Не удалось финишировать аукцион. Ошибки: \n {Join}", 
                    string.Join(Environment.NewLine, finishAuctionResult.Errors));
                return Result.Error(new ErrorList(finishAuctionResult.Errors));
            }
            
            return Result.Success();
        }
    }
    
    internal class Validator : AbstractValidator<Command>
    {
        public Validator(IReadRepository<Auction> repository)
        {
            RuleFor(x => x.Request.Id)
                .SetAsyncValidator(new AuctionExistsValidator<Command>(repository, x => x.Request.Id));
        }
    }
    
}