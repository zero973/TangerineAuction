namespace TangerineAuction.Infrastructure.Keycloak;

public interface IKeycloakTokenService
{
    Task<string> GetAccessToken(CancellationToken ct = default);
}