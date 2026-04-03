using Ardalis.Result;
using FluentValidation;
using MediatR;
using TangerineAuction.Core.Models;
using TangerineAuction.Core.Repository;
using TangerineAuction.Core.Services;
using TangerineAuction.Core.UseCases.Auctions.Dtos;
using TangerineAuction.Core.UseCases.Auctions.Specifications;
using TangerineAuction.Core.UseCases.Tangerines;

namespace TangerineAuction.Core.UseCases.Auctions;

public class GetAuctions
{
    
    public record Query(AuctionSearchParams SearchParams) : IRequest<Result<List<AuctionResponse>>>;

    internal class RequestHandler(IRepository<Auction> repository, ISender sender, ICurrentUserService currentUserService) 
        : IRequestHandler<Query, Result<List<AuctionResponse>>>
    {
        public async Task<Result<List<AuctionResponse>>> Handle(Query request, CancellationToken ct)
        {
            var result = (await repository.ListAsync(
                    new AuctionResponseSpecification().WithSearchParams(request.SearchParams, currentUserService.UserId), ct))
                .ToDictionary(x => x.TangerineFileName, x => x);

            if (result.Any())
            {
                var urls = await sender.Send(new GetTangerinePhotoUrls.Query(result.Keys.ToList()), ct);
                foreach (var url in urls.Value)
                    result[url.Key].ImageUrl = url.Value;
            }
            
            return result.Values.ToList();
        }
    }
    
    internal class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.SearchParams.Skip)
                .GreaterThan(-1)
                .WithMessage("Кол-во элементов для пропуска должно быть не отрицательным");
            
            RuleFor(x => x.SearchParams.Take)
                .GreaterThan(0)
                .WithMessage("Кол-во элементов для взятия должно быть больше 0");
        }
    }
    
}