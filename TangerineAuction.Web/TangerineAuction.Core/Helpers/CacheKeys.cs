namespace TangerineAuction.Core.Helpers;

internal static class CacheKeys
{
    public static string GetAuctionKey(Guid auctionId) => $"auction:{auctionId}";
}