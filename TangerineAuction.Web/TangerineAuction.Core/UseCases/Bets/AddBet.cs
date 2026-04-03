using Ardalis.Result;
using FluentValidation;
using MediatR;
using TangerineAuction.Core.Models;
using TangerineAuction.Core.Repository;
using TangerineAuction.Core.Services;
using TangerineAuction.Core.UseCases.Bets.Dtos;
using TangerineAuction.Core.UseCases.Bets.Notifications;
using TangerineAuction.Core.UseCases.Bets.Validators;

namespace TangerineAuction.Core.UseCases.Bets;

/// <summary>
/// Сохранить ставку в БД
/// </summary>
public class AddBet
{
    
    public record Command(BetRequest Request) : IRequest<Result<BetResponse>>;
    
    internal class RequestHandler(IRepository<Auction> repository, IPublisher publisher, ICurrentUserService currentUserService) 
        : IRequestHandler<Command, Result<BetResponse>>
    {
        public async Task<Result<BetResponse>> Handle(Command request, CancellationToken ct)
        {
            var userId = request.Request.CreatedBy ?? currentUserService.UserId;
            var bet = Bet.Create(request.Request.AuctionId, request.Request.Price, userId);
            var auction = await repository.GetByIdAsync(request.Request.AuctionId, ct);
            auction!.AddBet(bet);
            
            await repository.SaveChangesAsync(ct);
            
            await publisher.Publish(new BetAdded.Notification(request.Request.AuctionId), ct);
            
            return Result.Success(new BetResponse(bet));
        }
    }
    
    internal class Validator : AbstractValidator<Command>
    {
        public Validator(IReadRepository<Auction> repository, ISender sender)
        {
            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Request).SetValidator(new BetRequestValidator(repository));
            
            RuleFor(x => x.Request).MustAsync(async (x, ct) =>
                {
                    var result = await sender.Send(new CanCreateBet.Query(x.AuctionId), ct);
                    return result.IsSuccess;
                })
                .WithMessage("Ваша ставка является последней на этом аукционе");
        
            RuleFor(x => x.Request).MustAsync(async (x, ct) =>
                {
                    var bet = await sender.Send(new GetLastAuctionBet.Query(x.AuctionId), ct);
                    return bet.Status == ResultStatus.NotFound || bet.Value.Price < x.Price;
                })
                .WithMessage("Ваша ставка должна быть больше предыдущей ставки");
        }
    }
    
}