using FluentValidation.Validators;
using TangerineAuction.Core.Models;
using TangerineAuction.Core.Repository;
using TangerineAuction.Core.UseCases.Auctions.Specifications;

namespace TangerineAuction.Core.UseCases.Auctions.Validators;

internal class AuctionExistsValidator<T>(IReadRepository<Auction> repository, Func<T, Guid> selector)
    : AsyncPredicateValidator<T, Guid>(async (instance, _, _, ct)
        => await repository.AnyAsync(new AuctionSpecification().WithId(selector(instance)), ct))
{
    protected override string GetDefaultMessageTemplate(string errorCode)
        => "Аукцион с таким Id не существует";
}