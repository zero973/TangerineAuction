using FluentValidation;
using TangerineAuction.Core.Models;
using TangerineAuction.Core.Repository;
using TangerineAuction.Core.Services;
using TangerineAuction.Core.UseCases.Auctions;
using TangerineAuction.Core.UseCases.Auctions.Validators;
using TangerineAuction.Core.UseCases.Bets.Dtos;

namespace TangerineAuction.Core.UseCases.Bets.Validators;

internal class BetRequestValidator : AbstractValidator<BetRequest>
{
    public BetRequestValidator(IReadRepository<Auction> repository)
    {
        RuleFor(x => x.AuctionId)
            .SetAsyncValidator(new AuctionExistsValidator<BetRequest>(repository, x => x.AuctionId));
        
        RuleFor(x => x.Price)
            .InclusiveBetween(1, 2000000)
            .WithMessage("Ставка должна быть больше 0 и меньше 2 млн");
    }
}