namespace TangerineAuction.Core.UseCases.Auctions.Dtos;

public class AuctionSearchParams
{
    
    public int Skip { get; set; }
    
    public int Take { get; set; }

    public string? Name { get; set; }
    
}