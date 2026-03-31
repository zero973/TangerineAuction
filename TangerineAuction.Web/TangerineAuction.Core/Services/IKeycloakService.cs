namespace TangerineAuction.Core.Services;

public interface IKeycloakService
{
    Task<string> GetUserEmail(string userId, CancellationToken ct = default);
}