using Ardalis.Result;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using TangerineAuction.Core.Helpers;
using TangerineAuction.Core.Models;
using TangerineAuction.Core.Repository;
using TangerineAuction.Core.UseCases.Auctions.Dtos;
using TangerineAuction.Core.UseCases.Auctions.Specifications;
using TangerineAuction.Core.UseCases.Auctions.Validators;
using TangerineAuction.Core.UseCases.Tangerines;

namespace TangerineAuction.Core.UseCases.Auctions;

public class GetAuction
{
    
    public record Query(Guid Id) : IRequest<Result<AuctionFullResponse>>;

    internal class RequestHandler(
        IRepository<Auction> repository, 
        HybridCache cache, 
        ISender sender, 
        ILogger<RequestHandler> logger) 
        : IRequestHandler<Query, Result<AuctionFullResponse>>
    {
        public async Task<Result<AuctionFullResponse>> Handle(Query request, CancellationToken ct)
        {
            return await cache.GetOrCreateAsync<AuctionFullResponse>(
                CacheKeys.GetAuctionKey(request.Id), 
                async _ =>
                {
                    var auction = (await repository.FirstOrDefaultAsync(
                        new AuctionFullResponseSpecification().WithId(request.Id), ct))!;
                    var imageUrl = (await sender.Send(new GetTangerinePhotoUrls.Query([auction.Tangerine.FileName]), ct))
                        .Value.FirstOrDefault().Value;
                    
                    if (imageUrl != null)
                        auction.Tangerine.ImageUrl = imageUrl;
                    else
                        logger.LogError("Не удалось получить ссылку на картинку. Id мандарина '{TangerineId}'", auction.Tangerine.Id);
                    
                    return auction;
                }, 
                cancellationToken: ct);
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