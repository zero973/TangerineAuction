using Microsoft.Extensions.Options;
using TangerineAuction.Core.Services;
using TangerineAuction.Infrastructure.Keycloak.Models;

namespace TangerineAuction.Infrastructure.Keycloak;

internal sealed class KeycloakService(
    IKeycloakAdminApi api,
    IOptions<KeycloakOptions> options,
    IKeycloakTokenService tokenService)
    : IKeycloakService
{
    public async Task<string> GetUserEmail(string userId, CancellationToken ct = default)
    {
        var token = await tokenService.GetAccessToken(ct);

        var user = await api.GetUserByIdAsync(
            options.Value.Realm,
            userId,
            $"Bearer {token}",
            ct);

        return user!.Email!;
    }
}