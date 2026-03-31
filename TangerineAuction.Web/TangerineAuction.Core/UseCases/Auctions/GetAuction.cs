using Ardalis.Result;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using TangerineAuction.Core.Helpers;
using TangerineAuction.Core.Models;
using TangerineAuction.Core.Repository;
using TangerineAuction.Core.UseCases.Auctions.Dtos;
using TangerineAuction.Core.UseCases.Auctions.Specifications;
using TangerineAuction.Core.UseCases.Auctions.Validators;

namespace TangerineAuction.Core.UseCases.Auctions;

public class GetAuction
{
    
    public record Query(Guid Id) : IRequest<Result<AuctionFullResponse>>;

    internal class RequestHandler(IRepository<Auction> repository, HybridCache cache) 
        : IRequestHandler<Query, Result<AuctionFullResponse>>
    {
        public async Task<Result<AuctionFullResponse>> Handle(Query request, CancellationToken ct)
        {
            var auction = await cache.GetOrCreateAsync<AuctionFullResponse>(
                CacheKeys.GetAuctionKey(request.Id), 
                async _ => 
                    (await repository.FirstOrDefaultAsync(new AuctionFullResponseSpecification().WithId(request.Id), ct))!, 
                cancellationToken: ct);
            
            return auction;
        }
    }
    
    internal class Validator : AbstractValidator<Query>
    {
        public Validator(IReadRepository<Auction> repository)
        {
            RuleFor(x => x.Id)
                .SetAsyncValidator(new AuctionExistsValidator<Query>(repository, x => x.Id));
        }
    }
    
}