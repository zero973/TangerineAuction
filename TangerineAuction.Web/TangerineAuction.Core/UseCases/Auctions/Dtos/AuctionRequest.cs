namespace TangerineAuction.Core.UseCases.Auctions.Dtos;

public class AuctionRequest
{
    
    public required string Name { get; set; }

    public required Guid TangerineId { get; set; }
    
}