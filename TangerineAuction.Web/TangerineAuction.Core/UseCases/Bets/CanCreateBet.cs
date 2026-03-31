using Ardalis.Result;
using FluentValidation;
using MediatR;
using TangerineAuction.Core.Models;
using TangerineAuction.Core.Repository;
using TangerineAuction.Core.Services;
using TangerineAuction.Core.UseCases.Auctions;
using TangerineAuction.Core.UseCases.Auctions.Validators;

namespace TangerineAuction.Core.UseCases.Bets;

/// <summary>
/// Может ли пользователь сделать ставку
/// </summary>
/// <remarks>Пользователь может сделать ставку, если он до этого не делал ставки на этом аукционе</remarks>
public class CanCreateBet
{
    
    public record Query(Guid AuctionId) : IRequest<Result<bool>>;
    
    internal class RequestHandler : IRequestHandler<Query, Result<bool>>
    {
        public Task<Result<bool>> Handle(Query request, CancellationToken ct) => Task.FromResult(Result.Success(true));
    }
    
    internal class Validator : AbstractValidator<Query>
    {
        public Validator(IReadRepository<Auction> repository, ISender sender, ICurrentUserService currentUserService)
        {
            ClassLevelCascadeMode = CascadeMode.Stop;
            
            RuleFor(x => x.AuctionId)
                .SetAsyncValidator(new AuctionExistsValidator<Query>(repository, x => x.AuctionId));
            
            RuleFor(x => x.AuctionId).MustAsync(async (x, ct) =>
                {
                    var auction = await repository.GetByIdAsync(x, ct);
                    return auction!.IsActual;
                })
            .WithMessage("Этот аукцион завершён");
        
            RuleFor(x => x.AuctionId).MustAsync(async (x, ct) =>
                {
                    var bet = await sender.Send(new GetLastAuctionBet.Query(x), ct);
                    return bet.Status == ResultStatus.NotFound || bet.Value.CreatedBy != currentUserService.UserId;
                })
                .WithMessage("Ваша ставка является последней на этом аукционе");
        }
    }
    
}