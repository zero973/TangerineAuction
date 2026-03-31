using Refit;

namespace TangerineAuction.Infrastructure.Keycloak.Models;

internal sealed class KeycloakTokenRequest
{
    [AliasAs("client_id")]
    public string ClientId { get; init; } = string.Empty;

    [AliasAs("client_secret")]
    public string ClientSecret { get; init; } = string.Empty;

    [AliasAs("grant_type")]
    public string GrantType { get; init; } = "client_credentials";
}
