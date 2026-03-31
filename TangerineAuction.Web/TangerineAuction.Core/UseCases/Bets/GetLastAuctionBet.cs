using Ardalis.Result;
using FluentValidation;
using MediatR;
using TangerineAuction.Core.Models;
using TangerineAuction.Core.Repository;
using TangerineAuction.Core.UseCases.Auctions.Validators;
using TangerineAuction.Core.UseCases.Bets.Dtos;
using TangerineAuction.Core.UseCases.Bets.Specifications;

namespace TangerineAuction.Core.UseCases.Bets;

public class GetLastAuctionBet
{
    
    public record Query(Guid AuctionId) : IRequest<Result<BetResponse>>;

    internal class RequestHandler(IReadRepository<Bet> repository) : IRequestHandler<Query, Result<BetResponse>>
    {
        public async Task<Result<BetResponse>> Handle(Query request, CancellationToken ct)
        {
            var bet = await repository.FirstOrDefaultAsync(new BetSpecification(request.AuctionId).GetLastBet(), ct);
            return bet == null ? Result.NotFound() : new BetResponse(bet);
        }
    }
    
    internal class Validator : AbstractValidator<Query>
    {
        public Validator(IReadRepository<Auction> repository)
        {
            RuleFor(x => x.AuctionId)
                .SetAsyncValidator(new AuctionExistsValidator<Query>(repository, x => x.AuctionId));
        }
    }
    
}