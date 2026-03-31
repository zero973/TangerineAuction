using Ardalis.Result;
using FluentValidation;
using MediatR;
using TangerineAuction.Core.Models;
using TangerineAuction.Core.Repository;
using TangerineAuction.Core.UseCases.Auctions.Dtos;
using TangerineAuction.Core.UseCases.Auctions.Validators;

namespace TangerineAuction.Core.UseCases.Auctions;

/// <summary>
/// Сохранение аукциона в БД
/// </summary>
public class AddAuction
{
    
    public record Command(AuctionRequest Request) : IRequest<Result<Auction>>;
    
    internal class RequestHandler(IRepository<Auction> repository) : IRequestHandler<Command, Result<Auction>>
    {
        public async Task<Result<Auction>> Handle(Command request, CancellationToken ct)
        {
            var auction = Auction.Create(request.Request.Name, request.Request.TangerineId);
            return await repository.AddAsync(auction, ct);
        }
    }
    
    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Request).SetValidator(new AuctionRequestValidator());
        }
    }
    
}