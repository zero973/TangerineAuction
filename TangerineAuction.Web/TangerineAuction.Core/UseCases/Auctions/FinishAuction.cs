using Ardalis.Result;
using FluentValidation;
using MediatR;
using TangerineAuction.Core.Models;
using TangerineAuction.Core.Repository;
using TangerineAuction.Core.Services;
using TangerineAuction.Core.UseCases.Auctions.Notifications;
using TangerineAuction.Core.UseCases.Auctions.Validators;
using TangerineAuction.Core.UseCases.Bets;
using TangerineAuction.Core.UseCases.Bets.Specifications;
using TangerineAuction.Core.UseCases.Email;
using TangerineAuction.Core.UseCases.Microservices;

namespace TangerineAuction.Core.UseCases.Auctions;

/// <summary>
/// Завершение аукциона. Отправка письма победителю и удаление аукциона, ставок и мандарина из БД
/// </summary>
public class FinishAuction
{
    
    public record Command(Guid Id) : IRequest<Result>;
    
    internal class RequestHandler(
        IRepository<Auction> auctionRepository,
        IRepository<Tangerine> tangerineRepository,
        IReadRepository<Bet> betRepository,
        IKeycloakService keycloakService, 
        ISender sender, 
        IPublisher publisher) 
        : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken ct)
        {
            var auction = (await auctionRepository.GetByIdAsync(request.Id, ct))!;
            var betsCount = await betRepository.CountAsync(new BetSpecification(request.Id), ct);
            var tangerine = (await tangerineRepository.GetByIdAsync(auction.TangerineId, ct))!;
            
            // если кто-то принял участие, то отправляем чек и дизеймблим этот аукцион (не удаляем)
            if (betsCount > 1)
            {
                var lastBet = await sender.Send(new GetLastAuctionBet.Query(request.Id), ct);
            
                var mailAddress = await keycloakService.GetUserEmail(lastBet.Value.CreatedBy.ToString(), ct);
                var receiptText = $"""
                                   ЧЕК АУКЦИОНА

                                   Id аукциона: {auction.Id}
                                   Дата: {DateTime.UtcNow:dd.MM.yyyy HH:mm:ss} UTC
                                   Победитель: {mailAddress}

                                   Лот: {tangerine!.Name}
                                   Ставка победителя: {lastBet.Value.Price:C2}

                                   Спасибо за участие!
                                   """;
                var sendResult = await sender.Send(
                    new SendEmail.Command(
                        mailAddress, 
                        "Аукцион был завершён", 
                        receiptText, 
                        $"D:\\Pics\\{tangerine.FilePath}"), 
                    ct);
                if (!sendResult.IsSuccess)
                    return sendResult;
                
                auction.Disable();
                await auctionRepository.SaveChangesAsync(ct);
                
                return Result.Success();
            }
            
            await auctionRepository.DeleteAsync(auction, ct);
            await tangerineRepository.DeleteAsync(tangerine, ct);

            await sender.Send(new DeleteTangerineImage.Command(tangerine.FilePath), ct);
            
            await publisher.Publish(new AuctionFinishedNotification(request.Id), ct);
            
            return Result.Success();
        }
    }
    
    internal class Validator : AbstractValidator<Command>
    {
        public Validator(IReadRepository<Auction> repository)
        {
            RuleFor(x => x.Id)
                .SetAsyncValidator(new AuctionExistsValidator<Command>(repository, x => x.Id));
        }
    }
    
}