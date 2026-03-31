using Ardalis.Result;
using FluentValidation;
using MediatR;
using TangerineAuction.Core.Models;
using TangerineAuction.Core.Repository;
using TangerineAuction.Core.UseCases.Auctions.Dtos;
using TangerineAuction.Core.UseCases.Auctions.Specifications;

namespace TangerineAuction.Core.UseCases.Auctions;

public class GetAuctions
{
    
    public record Query(AuctionSearchParams SearchParams) : IRequest<Result<List<AuctionResponse>>>;

    internal class RequestHandler(IRepository<Auction> repository) : IRequestHandler<Query, Result<List<AuctionResponse>>>
    {
        public async Task<Result<List<AuctionResponse>>> Handle(Query request, CancellationToken ct) 
            => await repository.ListAsync(new AuctionResponseSpecification().WithSearchParams(request.SearchParams), ct);
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