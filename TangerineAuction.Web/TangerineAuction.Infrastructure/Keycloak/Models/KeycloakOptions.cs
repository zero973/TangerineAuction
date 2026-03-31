namespace TangerineAuction.Infrastructure.Keycloak.Models;

internal class KeycloakOptions
{
    public string Realm { get; init; } = string.Empty;
    public string AuthServerUrl { get; init; } = string.Empty;
    public string Resource { get; init; } = string.Empty;

    public KeycloakCredentials Credentials { get; init; } = new();
}